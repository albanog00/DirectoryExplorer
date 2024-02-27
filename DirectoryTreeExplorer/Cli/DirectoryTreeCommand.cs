using Spectre.Console;
using Spectre.Console.Cli;
using DirectoryTreeExplorer.DirectoryTree;
using System.ComponentModel;
using System.Text;

namespace DirectoryTreeExplorer.Cli;

public class DirectoryTreeCommand : Command<DirectoryTreeCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to search. Default to curren directory")]
        [CommandArgument(0, "[searchPath]")]
        public string SearchPath { get; init; } = Directory.GetCurrentDirectory();
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var directoryTreeInfo = new DirectoryTreeInfo(settings.SearchPath);

        while (directoryTreeInfo != null) {
            var tree = directoryTreeInfo.RenderTree().Split("\n");
            var header = RenderHeader(ref directoryTreeInfo);

            var selectedDirectory = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .PageSize(20)
                .Title(header)
                .AddChoices(tree)
                );

            directoryTreeInfo = new(directoryTreeInfo.FullName + selectedDirectory);

        }
        return 0;
    }

    public string RenderHeader(ref DirectoryTreeInfo directoryTreeInfo)
    {
        var builder = new StringBuilder();
        builder.Append('\n');
        builder.Append($"Current directory: {directoryTreeInfo.FullName}");
        builder.Append(string.Format("\n{0} [bold][yellow]Directories[/][/], {1} [bold][dodgerblue2]Files[/][/], [bold][green]{2}[/][/] allocated",
            directoryTreeInfo.DirectoryCount, directoryTreeInfo.FilesCount, directoryTreeInfo.FormatLength()));

        return builder.ToString();
    }
}