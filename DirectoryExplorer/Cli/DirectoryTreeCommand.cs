using Spectre.Console;
using Spectre.Console.Cli;
using DirectoryTreeExplorer.DirectoryTree;
using System.ComponentModel;
using System.Text;

namespace DirectoryTreeExplorer.Cli;

public class DirectoryTreeCommand : Command<DirectoryTreeCommand.Settings>
{
    private const string OpenFolderEmoji = Emoji.Known.OpenFileFolder;
    private const string FolderEmoji = Emoji.Known.FileFolder;
    private const string FileEmoji = Emoji.Known.PageFacingUp;

    public class Settings : CommandSettings
    {
        [Description("Path to search. Default to curren directory")]
        [CommandArgument(0, "[searchPath]")]
        public string SearchPath { get; init; } = Directory.GetCurrentDirectory();
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Clear();

        var directoryTreeInfo = new DirectoryTreeInfo(settings.SearchPath);
        while (directoryTreeInfo != null) {
            var header = RenderHeader(ref directoryTreeInfo);
            var tree = directoryTreeInfo.RenderTree().Split("\n");

            var prompt = new SelectionPrompt<string>() {
                Title = header,
                PageSize = 20,
                Converter = a => (
                    a[0] == '/'
                        ? $"{OpenFolderEmoji} [yellow]{Markup.Escape(a)}[/]"
                        : $"{FileEmoji} [dodgerblue2]{Markup.Escape(a)}[/] - [green]{FormatLength(directoryTreeInfo.GetFileLength(a))}[/]"
                ),
            }.AddChoices(tree);

            var directoryName = AnsiConsole.Prompt(prompt);
            if (directoryName[0] != '/')
                continue;

            directoryName =
                directoryName[0..3] == "/.."
                    ? directoryTreeInfo.GetParentFullName()
                    : directoryTreeInfo.FullName + directoryName;
            if (string.IsNullOrEmpty(directoryName))
                continue;

            AnsiConsole.Clear();
            directoryTreeInfo = new DirectoryTreeInfo(directoryName);
        }
        return 0;
    }

    public string RenderHeader(ref DirectoryTreeInfo directoryTreeInfo)
    {
        var builder = new StringBuilder();
        builder
            .AppendFormat(
                "\n[bold]Current directory: {0} [lightgoldenrod2_1]{1}[/][/]\n",
                FolderEmoji,
                directoryTreeInfo.FullName)
            .AppendFormat(
                "{0} {1} [bold][yellow]Directories[/][/], {2} {3} [bold][dodgerblue2]Files[/][/], [bold][green]{4}[/][/] allocated",
                OpenFolderEmoji,
                directoryTreeInfo.DirectoryCount,
                FileEmoji,
                directoryTreeInfo.FilesCount,
                FormatLength(directoryTreeInfo.Length));

        return builder.ToString();
    }

    public static string FormatLength(long length)
    {
        string format;
        if (length >= 0 && length <= 1023)
            format = $"{string.Format("{0:0.00}", length)} byte(s)";
        else if (length >= 1024 && length <= 1_048_575)
            format = $"{string.Format("{0:0.00}", (double)(length) / 1024)} kb(s)";
        else if (length >= 1_048_576 && length <= 1_073_741_823)
            format = $"{string.Format("{0:0.00}", (double)(length) / 1_048_576)} mb(s)";
        else if (length >= 1_073_741_824 && length <= 1_099_511_627_775)
            format = $"{string.Format("{0:0.00}", (double)(length) / 1_073_741_824)} gb(s)";
        else format = $"{string.Format("{0:0.00}", (double)(length) / 1_099_511_627_776)} tb(s)";

        return format;
    }
}