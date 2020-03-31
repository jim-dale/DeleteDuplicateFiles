using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace DeleteDuplicateFiles
{
    internal class AllDirectories
    {
        private readonly Options _options;
        private readonly Summary _summary;

        public AllDirectories(Options options, Summary summary)
        {
            _options = options;
            _summary = summary;
        }

        public void Run(Action<IEnumerable<FileInfo>, Options, Summary> action)
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
                .AddIncludePatterns(_options.Include, ';', "*")
                .AddExcludePatterns(_options.Exclude, ';');

            var matches = matcher.GetResultsInFullPath(_options.Path);

            var files = from m in matches
                        let fi = new FileInfo(m)
                        orderby fi.Length
                        select fi;

            if (files.Any())
            {
                action?.Invoke(files, _options, _summary);
            }
        }
    }
}
