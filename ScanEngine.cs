using System.IO;
namespace OpenSpace;
public sealed record SpaceEntry(string Path, long Bytes, bool IsDirectory);
public static class ScanEngine
{
    public static IReadOnlyList<SpaceEntry> Scan(string root, CancellationToken cancellationToken = default, IProgress<SpaceEntry>? progress = null)
    {
        if (!Directory.Exists(root)) throw new DirectoryNotFoundException(root); var results = new List<SpaceEntry>();
        foreach (var path in Directory.EnumerateFileSystemEntries(root, "*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested(); try { var info = File.GetAttributes(path); if ((info & FileAttributes.ReparsePoint) != 0) continue; var bytes = (info & FileAttributes.Directory) != 0 ? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Sum(SafeLength) : new FileInfo(path).Length; var entry = new SpaceEntry(path, bytes, (info & FileAttributes.Directory) != 0); results.Add(entry); progress?.Report(entry); } catch (UnauthorizedAccessException) { }
        }
        return results.OrderByDescending(x => x.Bytes).ToArray();
    }
    private static long SafeLength(string path) { try { return new FileInfo(path).Length; } catch { return 0; } }
}
