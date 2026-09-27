using System.IO;
namespace OpenSpace;
public sealed record SpaceEntry(string Path, long Bytes, bool IsDirectory);
public static class ScanEngine
{
    public static IReadOnlyList<SpaceEntry> Scan(string root, CancellationToken cancellationToken = default, IProgress<SpaceEntry>? progress = null)
    {
        if (!Directory.Exists(root)) throw new DirectoryNotFoundException(root); var files = new List<(string Path, long Bytes)>();
        foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)) { cancellationToken.ThrowIfCancellationRequested(); try { var info = new FileInfo(path); if ((info.Attributes & FileAttributes.ReparsePoint) != 0) continue; files.Add((path, info.Length)); progress?.Report(new SpaceEntry(path, info.Length, false)); } catch (UnauthorizedAccessException) { } }
        var result = files.Select(x => new SpaceEntry(x.Path, x.Bytes, false)).ToList(); var directories = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in files) { var current = Directory.GetParent(file.Path)?.FullName; while (current is not null && current.StartsWith(Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase)) { directories[current] = directories.GetValueOrDefault(current) + file.Bytes; current = Directory.GetParent(current)?.FullName; } }
        result.AddRange(directories.Select(x => new SpaceEntry(x.Key, x.Value, true))); return result.OrderByDescending(x => x.Bytes).ToArray();
    }
}
