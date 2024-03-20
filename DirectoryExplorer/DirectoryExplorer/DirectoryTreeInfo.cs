using Spectre.Console;
using System.Text;

namespace DirectoryTreeExplorer.DirectoryTree;

public class DirectoryTreeInfo
{
    private readonly DirectoryInfo _directoryInfo;
    private readonly HashSet<string> _directories = [];
    private readonly Dictionary<string, long> _files = [];

    public string FullName { get; private set; }
    public string Name { get; private set; }
    public long Length { get; private set; }

    public int DirectoryCount => _directories.Count;
    public int FilesCount => _files.Count;

    public DirectoryTreeInfo(string fullName)
    {
        _directoryInfo = new DirectoryInfo(fullName);
        FullName = _directoryInfo.FullName;
        Name = _directoryInfo.Name;
        Init();
    }

    private void Init()
    {
        try
        {
            foreach (var directory in _directoryInfo.GetDirectories())
            {
                _directories.Add(directory.Name);
            }

            foreach (var file in _directoryInfo.GetFiles())
            {
                _files[file.Name] = file.Length;
                Length += file.Length;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(new Exception(ex.Message));
        }
    }

    public string RenderTree()
    {
        var builder = new StringBuilder();

        if (_directoryInfo.Parent is not null)
            builder.Append("/..\n");
        foreach (var directory in _directories)
            builder.AppendFormat("/{0}\n", directory);
        foreach (var (key, _) in _files)
            builder.AppendFormat("{0}\n", key);

        return builder.ToString().TrimEnd('\n');
    }

    public string? GetParentFullName() => _directoryInfo.Parent?.FullName;

    public long GetFileLength(string name) => _files[name];
}
