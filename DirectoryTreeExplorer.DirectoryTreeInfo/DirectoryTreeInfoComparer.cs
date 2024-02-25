namespace DirectoryTreeExplorer.DirectoryTreeInfo
{
    internal class DirectoryTreeInfoComparer : IComparer<DirectoryTreeInfo>
    {
        public int Compare(DirectoryTreeInfo? x, DirectoryTreeInfo? y) => (int)(x!.Length - y!.Length);
    }
}
