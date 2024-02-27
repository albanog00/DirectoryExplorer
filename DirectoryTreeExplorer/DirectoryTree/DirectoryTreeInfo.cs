using Spectre.Console;
using System.Text;

namespace DirectoryTreeExplorer.DirectoryTree;

public class DirectoryTreeInfo
{
    private readonly byte _maxDepth;
    private readonly byte _currentDepth;

    private readonly DirectoryInfo _directoryInfo;
    private readonly HashSet<DirectoryTreeInfo> _subDirectories = [];

    public string FullName { get; private set; }
    public string Name { get; private set; }
    public long Length { get; private set; }

    private DirectoryTreeInfo(string fullName, byte maxDepth, byte currentDepth)
    {
        FullName = fullName;
        _directoryInfo = new DirectoryInfo(fullName);

        Name = _directoryInfo.Name;

        _maxDepth = maxDepth;
        _currentDepth = currentDepth;

        try {
            foreach (var fileInfo in _directoryInfo.GetFiles())
                Length += fileInfo.Length;

            var directories = _directoryInfo.GetDirectories();
            for (int i = 0; _maxDepth > 0 && i < directories.Length; ++i) {
                var nextNode = new DirectoryTreeInfo(
                    directories[i].FullName,
                    (byte)(_maxDepth - 1),
                    (byte)(_currentDepth + 1));

                _subDirectories.Add(nextNode);
                Length += nextNode.Length;
            }
        }
        catch (Exception ex) {
            AnsiConsole.MarkupLine($"[red]There's been an error:[/] {ex.Message}");
        }
    }

    public DirectoryTreeInfo(string fullName, byte maxDepth = byte.MaxValue)
        : this(fullName, maxDepth, 0) { }

    public string Render()
    {
        var builder = new StringBuilder();

        Stack<DirectoryTreeInfo> stack = [];
        stack.Push(this);

        while (stack.Count > 0) {
            var currentDirectory = stack.Pop();
            foreach (var _ in Enumerable.Range(0, currentDirectory._currentDepth))
                builder.Append("|---- ");
            builder.Append($"[grey78]{currentDirectory.Name}[/] - {currentDirectory.FormatLength()}\n");

            foreach (var directory in currentDirectory._subDirectories)
                stack.Push(directory);
        }

        return builder.ToString();
    }

    public string RenderWithFiles()
    {
        var builder = new StringBuilder();

        Stack<DirectoryTreeInfo> stack = [];
        stack.Push(this);

        while (stack.Count > 0) {
            var currentDirectory = stack.Pop();
            foreach (var _ in Enumerable.Range(0, currentDirectory._currentDepth))
                builder.Append("|---- ");
            builder.Append($"[grey78]{currentDirectory.Name}[/] - {currentDirectory.FormatLength()}\n");

            try {
                foreach (var file in currentDirectory._directoryInfo.GetFiles()) {
                    foreach (var _ in Enumerable.Range(0, _currentDepth + 1))
                        builder.Append("|---- ");
                    builder.Append($"[grey46]{file.Name}[/] - {FormatLength(file.Length)}\n");
                }
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red]There's been an error:[/] {ex.Message}");
            }

            foreach (var directory in currentDirectory._subDirectories)
                stack.Push(directory);
        }

        return builder.ToString();
    }

    public string FormatLength() => FormatLength(Length);

    private string FormatLength(long length)
    {
        string format;
        if (length >= 0 && length <= 1023)
            format = $"{string.Format("{0:0.00}", length)} byte(s)";
        else if (length >= 1024 && length <= 1_048_575)
            format = $"{string.Format("{0:0.00}", length / 1024)} kb(s)";
        else if (length >= 1_048_576 && length <= 1_073_741_823)
            format = $"{string.Format("{0:0.00}", length / 1_048_576)} mb(s)";
        else if (length >= 1_073_741_824 && length <= 1_099_511_627_775)
            format = $"{string.Format("{0:0.00}", length / 1_073_741_824)} gb(s)";
        else format = $"{string.Format("{0:0.00}", length / 1_099_511_627_776)} tb(s)";

        return $"[green]{format}[/]";
    }
}