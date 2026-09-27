using System.IO;
using System.Windows;
using Forms = System.Windows.Forms;
using TextBlock = System.Windows.Controls.TextBlock;
namespace OpenSpace;
public partial class MainWindow : Window
{
    private string? root; private CancellationTokenSource? scanCts; private readonly System.Windows.Controls.ProgressBar largestBar = new() { Height = 10, Minimum = 0, Maximum = 100, Margin = new Thickness(0, 8, 0, 18) }; private readonly TextBlock largestText = new() { Foreground = System.Windows.Media.Brushes.Gray };
    public MainWindow() { InitializeComponent(); if (TotalText.Parent is System.Windows.Controls.Panel panel) { largestText.Text = "Largest location"; panel.Children.Insert(3, largestText); panel.Children.Insert(4, largestBar); } }
    private void ChooseFolder_Click(object sender, RoutedEventArgs e) { using var dialog = new Forms.FolderBrowserDialog { Description = "Choose a folder to analyze" }; if (dialog.ShowDialog() == Forms.DialogResult.OK) { root = dialog.SelectedPath; PathText.Text = root; StatusText.Text = "Ready to scan"; } }
    private async void Scan_Click(object sender, RoutedEventArgs e) { if (root is null) return; scanCts?.Cancel(); scanCts = new CancellationTokenSource(); ScanButton.IsEnabled = false; CancelButton.IsEnabled = true; StatusText.Text = "Scanning…"; try { var progress = new Progress<SpaceEntry>(entry => StatusText.Text = $"Scanning {Path.GetFileName(entry.Path)}"); var entries = await Task.Run(() => ScanEngine.Scan(root, scanCts.Token, progress), scanCts.Token); EntryList.ItemsSource = entries.Take(100).Select(x => $"{(x.IsDirectory ? "▣" : "•")}  {Path.GetFileName(x.Path)}    {Format(x.Bytes)}").ToArray(); var files = entries.Where(x => !x.IsDirectory).ToArray(); var total = files.Sum(x => x.Bytes); var largest = entries.FirstOrDefault(); TotalText.Text = Format(total); largestText.Text = largest is null ? "Largest location" : $"Largest: {Path.GetFileName(largest.Path)}  ·  {Format(largest.Bytes)}"; largestBar.Value = total == 0 || largest is null ? 0 : Math.Min(100, largest.Bytes * 100d / total); CountText.Text = $"{entries.Count:N0} entries"; StatusText.Text = "Scan complete"; } catch (OperationCanceledException) { StatusText.Text = "Scan cancelled"; } catch (Exception ex) { StatusText.Text = ex.Message; } finally { ScanButton.IsEnabled = true; CancelButton.IsEnabled = false; } }
    private void Cancel_Click(object sender, RoutedEventArgs e) => scanCts?.Cancel();
    private static string Format(long bytes) => bytes switch { >= 1_000_000_000 => $"{bytes / 1_000_000_000d:0.0} GB", >= 1_000_000 => $"{bytes / 1_000_000d:0.0} MB", >= 1_000 => $"{bytes / 1_000d:0.0} KB", _ => $"{bytes:N0} B" };
}
