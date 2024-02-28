using DirectoryTreeExplorer.Cli;
using Spectre.Console.Cli;

var app = new CommandApp<DirectoryTreeCommand>();
app.Run(args);
