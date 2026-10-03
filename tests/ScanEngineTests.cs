using OpenSpace;
var root = Path.Combine(Path.GetTempPath(), "openspace-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(Path.Combine(root, "nested")); File.WriteAllText(Path.Combine(root, "small.txt"), "small"); File.WriteAllBytes(Path.Combine(root, "nested", "large.bin"), new byte[4096]);
var progressItems = new List<SpaceEntry>(); var report = ScanEngine.ScanDetailed(root, progress: new InlineProgress(progressItems)); var results = report.Entries;
if (!results.Any(x => x.Path.EndsWith("large.bin") && x.Bytes == 4096) || results.First().Bytes < 4096) throw new Exception("Scan did not rank file sizes correctly.");
if (report.FilesScanned != 2 || report.TotalBytes != 4101 || progressItems.Count != 2) throw new Exception("Scan summary or progress is incorrect.");
var rootEntry = results.Single(x => x.IsDirectory && x.Path.Equals(Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase)); if (rootEntry.Bytes != 4101) throw new Exception("Root aggregation is incorrect.");
using var cts = new CancellationTokenSource(); cts.Cancel(); try { ScanEngine.Scan(root, cts.Token); throw new Exception("Cancellation was ignored."); } catch (OperationCanceledException) { }
Directory.Delete(root, true); Console.WriteLine("PASS: fault-tolerant read-only scan, exact aggregation, progress, and cancellation");

sealed class InlineProgress(List<SpaceEntry> items) : IProgress<SpaceEntry> { public void Report(SpaceEntry value) => items.Add(value); }
