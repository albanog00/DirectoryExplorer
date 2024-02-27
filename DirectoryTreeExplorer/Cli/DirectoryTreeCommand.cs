using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;
using DirectoryTreeExplorer.DirectoryTree;
using System.ComponentModel;

namespace DirectoryTreeExplorer.Cli;

public class DirectoryTreeCommand : Command<DirectoryTreeCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to search. Default to %HOMEPATH%.")]
        [CommandArgument(0, "[searchPath]")]
        public string SearchPath { get; init; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        [Description("Max depth of the directory tree. Default is 255")]
        [CommandOption("-d|--depth")]
        public byte Depth { get; init; } = byte.MaxValue;

        [Description("Include files in directories. Default false.")]
        [CommandOption("-f|--files")]
        public bool IncludeFiles { get; init; } = false;

        [Description("Path to write file.")]
        [CommandOption("-o|--output")]
        public string? OutputPath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var clock = new Stopwatch();
        clock.Start();

        var directoryTreeInfo = new DirectoryTreeInfo(settings.SearchPath, settings.Depth);

        string result = settings.IncludeFiles ? directoryTreeInfo.RenderWithFiles() : directoryTreeInfo.Render();

        if (settings.OutputPath != null) {
            try {
                using StreamWriter writer = new(settings.OutputPath);
                writer.WriteLine(result);
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine(string.Format("[red]Can't write to {0}: {1}[/]", settings.OutputPath, ex.ToString()));
            }
        }
        else {
            AnsiConsole.MarkupLine(result);
        }

        clock.Stop();
        AnsiConsole.MarkupLine($"\nProgram took [green]{string.Format("{0:0.00}", clock.Elapsed.TotalSeconds)}[/]s to execute.");

        return 0;
    }
}