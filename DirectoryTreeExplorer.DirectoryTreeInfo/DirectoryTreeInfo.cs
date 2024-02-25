using System.Text;

namespace DirectoryTreeExplorer.DirectoryTreeInfo
{
    public class DirectoryTreeInfo
    {
        private DirectoryTreeInfo(string fullName, string name, sbyte maxDepth, sbyte currentDepth)
        {
            FullName = fullName;
            Name = name;
            _maxDepth = maxDepth;
            _currentDepth = currentDepth;

            _directoryInfo = new DirectoryInfo(fullName);
            GetDirectoryInfo();
        }

        public DirectoryTreeInfo(string fullName, sbyte maxDepth = sbyte.MaxValue)
            : this(fullName, fullName.Split(@"\").Last(), maxDepth, 0) { }

        private readonly sbyte _maxDepth;
        private readonly sbyte _currentDepth;

        private readonly DirectoryInfo _directoryInfo;
        private readonly HashSet<DirectoryTreeInfo> _subDirectories = [];
        private readonly Dictionary<string, HashSet<DirectoryInfo>> _directoryTree = [];

        public string FullName { get; private set; }
        public string Name { get; private set; }
        public long Length { get; private set; }

        public void PrintTree(bool includeFiles = false)
        {
            var builder = new StringBuilder();

            PrintTreeHelper(builder, includeFiles);
            Console.WriteLine(builder.ToString());
        }

        private void PrintTreeHelper(StringBuilder builder, bool includeFiles)
        {
            builder.Append("\n ");
            foreach (var _ in Enumerable.Range(0, _currentDepth))
                builder.Append("|---- ");
            builder.Append($"{Name} - {FormatLength()}");

            if (includeFiles)
            {
                try
                {
                    foreach (var file in _directoryInfo.GetFiles())
                        PrintFileTreeHelper(builder, file);
                }
            catch (Exception) { /* Console.WriteLine($"There's been an error: {ex.Message}"); */ }
            }

            foreach (var directory in _subDirectories)
                directory.PrintTreeHelper(builder, includeFiles);
        }

        private void PrintFileTreeHelper(StringBuilder builder, FileInfo file)
        {
            builder.Append("\n ");
            foreach (var _ in Enumerable.Range(0, _currentDepth + 1))
                builder.Append("|---- ");
            builder.Append($"{file.Name} - {FormatLength(file.Length)}");
        }

        private void GetDirectoryInfo()
        {
            try
            {
                GetCurrentDirectorySize();

                var directories = _directoryInfo.GetDirectories();
                for (int i = 0; _maxDepth > 0 && i < directories.Length; i++)
                {
                    var nextNode = new DirectoryTreeInfo(
                        directories[i].FullName,
                        directories[i].Name,
                        (sbyte)(_maxDepth - 1),
                        (sbyte)(_currentDepth + 1));

                    _subDirectories.Add(nextNode);

                    Length += nextNode.Length;
                }
            }
            catch (Exception) { /* Console.WriteLine($"There's been an error: {ex.Message}"); */ }
        }

        private void GetCurrentDirectorySize()
        {
            foreach (var fileInfo in _directoryInfo.GetFiles())
                Length += fileInfo.Length;
        }

        private string FormatLength(long length)
        {
            string format;
            if (length >= 0 && length <= 1023)
                format = $"{String.Format("{0:0.00}", length)} byte(s)";
            else if (length >= 1024 && length <= 1_048_575)
                format = $"{String.Format("{0:0.00}", length / 1024)} kb(s)";
            else if (length >= 1_048_576 && length <= 1_073_741_823)
                format = $"{String.Format("{0:0.00}", length / 1_048_576)} mb(s)";
            else if (length >= 1_073_741_824 && length <= 1_099_511_627_775)
                format = $"{String.Format("{0:0.00}", length / 1_073_741_824)} gb(s)";
            else format = $"{String.Format("{0:0.00}", length / 1_099_511_627_776)} tb(s)";

            return $"[{format}]";
        }

        public string FormatLength()
        {
            string format;
            if (Length >= 0 && Length <= 1023)
                format = $"{String.Format("{0:0.00}", Length)} byte(s)";
            else if (Length >= 1024 && Length <= 1_048_575)
                format = $"{String.Format("{0:0.00}", Length / 1024)} kb(s)";
            else if (Length >= 1_048_576 && Length <= 1_073_741_823)
                format = $"{String.Format("{0:0.00}", Length / 1_048_576)} mb(s)";
            else if (Length >= 1_073_741_824 && Length <= 1_099_511_627_775)
                format = $"{String.Format("{0:0.00}", Length / 1_073_741_824)} gb(s)";
            else format = $"{String.Format("{0:0.00}", Length / 1_099_511_627_776)} tb(s)";

            return $"[{format}]";
        }
    }
}