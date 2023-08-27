namespace DeleteDuplicateFiles;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

internal class AllDirectories
{
    private readonly Options options;
    private readonly Summary summary;

    public AllDirectories(Options options, Summary summary)
    {
        this.options = options;
        this.summary = summary;
    }

    public void Run(Action<IEnumerable<FileInfo>, Options, Summary> action)
    {
        Matcher matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
            .AddIncludePatterns(this.options.Include, ';', "*")
            .AddExcludePatterns(this.options.Exclude, ';');

        IEnumerable<string> matches = matcher.GetResultsInFullPath(this.options.Path);

        IEnumerable<FileInfo> files = from m in matches
                    let fi = new FileInfo(m)
                    orderby fi.Length
                    select fi;

        if (files.Any())
        {
            action?.Invoke(files, this.options, this.summary);
        }
    }
}
