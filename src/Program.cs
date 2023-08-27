namespace DeleteDuplicateFiles;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

internal static class Program
{
    internal static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(Execute)
            .WithNotParsed(HandleParseError);
    }

    private static void Execute(Options options)
    {
        if (options.All)
        {
            ExecuteAllDirectories(options);
        }
        else
        {
            ExecutePerDirectory(options);
        }
    }
    
    private static void ExecuteAllDirectories(Options options)
    {
        var summary = new Summary(options.DeleteDuplicates == false);

        var processor = new AllDirectories(options, summary);

        processor.Run(ProcessFiles);

        if (options.ShowSummary)
        {
            summary.Show();
        }
    }

    private static void ExecutePerDirectory(Options options)
    {
        var summary = new Summary(options.DeleteDuplicates == false);

        var processor = new PerDirectory(options, summary);

        processor.Run(ProcessFiles);

        if (options.ShowSummary)
        {
            summary.Show();
        }
    }

    private static void ProcessFiles(IEnumerable<FileInfo> items, Options options, Summary summary)
    {
        if (options.Verbose)
        {
            items.Show();
        }

        IEnumerable<IGrouping<long, FileInfo>> sizeGroups = items.GroupByFileLength()
            .FilterByGroupSize(1);

        if (options.Verbose)
        {
            sizeGroups.Show();
        }

        IEnumerable<IGrouping<string, FileInfo>> hashGroups = sizeGroups.GroupByHash()
            .FilterByGroupSize(1);

        if (options.Verbose)
        {
            hashGroups.Show();
        }

        foreach (IGrouping<string, FileInfo> hashGroup in hashGroups)
        {
            string hash = hashGroup.Key;

            IOrderedEnumerable<FileInfo> orderedItems = OrderFilesByPriority(hashGroup);

            FileInfo retainedItem = orderedItems.First();

            Console.WriteLine($"Hash={hash}\\Count={orderedItems.Count()}");
            retainedItem.Show("Retain");

            foreach (FileInfo? item in orderedItems.Skip(1))
            {
                item.Show("Delete");

                if (options.DeleteDuplicates)
                {
                    item.Delete();
                }
                summary.RegisterFileDeletion(item.Length);
            }
        }
    }

    private static IOrderedEnumerable<FileInfo> OrderFilesByPriority(IEnumerable<FileInfo> items)
    {
        return from fi in items
               orderby fi.CreationTimeUtc, fi.FullName
               select fi;
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        //TODO: Show errors
    }
}
