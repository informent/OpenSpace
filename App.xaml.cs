using System.Windows;
namespace OpenSpace;
public partial class App : System.Windows.Application { protected override void OnStartup(StartupEventArgs e) { base.OnStartup(e); MainWindow = new MainWindow(); MainWindow.Show(); } }
