using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace DeleteDuplicateFiles
{
    internal class PerDirectory
    {
        private readonly Options _options;
        private readonly Summary _summary;
        private readonly Matcher _matcher;

        public PerDirectory(Options options, Summary summary)
        {
            _options = options;
            _summary = summary;

            _matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
                    .AddIncludePatterns(options.Include, ';', "*")
                    .AddExcludePatterns(options.Exclude, ';');
        }

        public void Run(Action<IEnumerable<FileInfo>, Options, Summary> action)
        {
            ProcessDirectory(new DirectoryInfo(_options.Path), action);
        }

        private void ProcessDirectory(DirectoryInfo value, Action<IEnumerable<FileInfo>, Options, Summary> action)
        {
            var files = from file in value.EnumerateFiles()
                        where _matcher.Match(file.Name).HasMatches
                        orderby file.Length
                        select file;

            if (files.Any())
            {
                action?.Invoke(files, _options, _summary);
            }

            var directories = value.EnumerateDirectories();
            foreach (var directory in directories)
            {
                ProcessDirectory(directory, action);
            }
        }
    }
}
