namespace DeleteDuplicateFiles;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

internal class PerDirectory
{
    private readonly Options options;
    private readonly Summary summary;
    private readonly Matcher matcher;

    public PerDirectory(Options options, Summary summary)
    {
        this.options = options;
        this.summary = summary;

        this.matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
                .AddIncludePatterns(options.Include, ';', "*")
                .AddExcludePatterns(options.Exclude, ';');
    }

    public void Run(Action<IEnumerable<FileInfo>, Options, Summary> action)
    {
        this.ProcessDirectory(new DirectoryInfo(this.options.Path), action);
    }

    private void ProcessDirectory(DirectoryInfo value, Action<IEnumerable<FileInfo>, Options, Summary> action)
    {
        IOrderedEnumerable<FileInfo> files = from file in value.EnumerateFiles()
                                             where this.matcher.Match(file.Name).HasMatches
                                             orderby file.Length
                                             select file;

        if (files.Any())
        {
            action.Invoke(files, this.options, this.summary);
        }

        IEnumerable<DirectoryInfo> directories = value.EnumerateDirectories();
        foreach (DirectoryInfo directory in directories)
        {
            this.ProcessDirectory(directory, action);
        }
    }
}
