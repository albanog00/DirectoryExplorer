using Spectre.Console;
using System.Text;

namespace DirectoryTreeExplorer.DirectoryTree;

public class DirectoryTreeInfo
{
    private readonly DirectoryInfo _directoryInfo;

    public string FullName { get; private set; }
    public string Name { get; private set; }

    public long Length { get; private set; }
    public int DirectoryCount { get; private set; }
    public int FilesCount { get; private set; }

    public DirectoryTreeInfo(string fullName)
    {
        _directoryInfo = new DirectoryInfo(fullName);
        FullName = _directoryInfo.FullName;
        Name = _directoryInfo.Name;
    }

    public string RenderTree()
    {
        var builder = new StringBuilder();

        if (_directoryInfo.Parent is not null)
            builder.Append("\\..");

        try {
            foreach (var directory in _directoryInfo.GetDirectories()) {
                builder.Append($"\n\\{Markup.Escape(directory.Name)}");
                ++DirectoryCount;
            }

            foreach (var file in _directoryInfo.GetFiles()) {
                //builder.AppendLine($"{Markup.Escape(file.Name)} - {FormatLength(file.Length)}");
                Length += file.Length;
                ++FilesCount;
            }
        }
        catch (Exception ex) {
            AnsiConsole.MarkupLine($"[red]There's been an error:[/] {ex.Message}");
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
            format = $"{string.Format("{0:0.00}", (double)(length / 1024))} kb(s)";
        else if (length >= 1_048_576 && length <= 1_073_741_823)
            format = $"{string.Format("{0:0.00}", (double)(length / 1_048_576))} mb(s)";
        else if (length >= 1_073_741_824 && length <= 1_099_511_627_775)
            format = $"{string.Format("{0:0.00}", (double)(length / 1_073_741_824))} gb(s)";
        else format = $"{string.Format("{0:0.00}", (double)(length / 1_099_511_627_776))} tb(s)";

        return format;
    }
}