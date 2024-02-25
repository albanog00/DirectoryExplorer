using DirectoryTreeExplorer.DirectoryTreeInfo;
using System.Diagnostics;

var clock = new Stopwatch();

clock.Start();
var directoryTreeInfo = new DirectoryTreeInfo(@"C:\Users\Giuseppe");

directoryTreeInfo.PrintTree(true);

clock.Stop();

Console.WriteLine($"\nProgram took {clock.Elapsed.TotalSeconds}s to execute.");
