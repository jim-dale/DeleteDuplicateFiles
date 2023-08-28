namespace DeleteDuplicateFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using DeleteDuplicateFiles.Helpers;
using DeleteDuplicateFiles.Models;

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
        var summary = new Summary(options.DeleteDuplicates == false);

        var processor = new FileFinder(options);

        IEnumerable<DedupFileInfo> items = processor.GetItems();

        ProcessFiles(items, options, summary);

        if (options.ShowSummary)
        {
            summary.Show();
        }
    }

    private static void ProcessFiles(IEnumerable<DedupFileInfo> items, Options options, Summary summary)
    {
        if (options.Verbose)
        {
            items.Show();
        }

        IEnumerable<IGrouping<string, DedupFileInfo>> groups = items.GroupBy(x => x.HashString).Where(g => g.Count() > 1);

        if (options.Verbose)
        {
            groups.Show();
        }

        foreach (IGrouping<string, DedupFileInfo> group in groups)
        {
            string hash = group.Key;

            IOrderedEnumerable<DedupFileInfo> orderedItems = group.OrderBy(i => i.FileNameLength).ThenBy(i => i.CreationTimeUtc);

            DedupFileInfo itemToBeRetained = orderedItems.First();

            Console.WriteLine($"Hash={hash}\\Count={orderedItems.Count()}");
            itemToBeRetained.Show("Retain");

            DedupFileInfo[] itemsToBeDeleted = orderedItems.Skip(1).ToArray();

            foreach (DedupFileInfo item in itemsToBeDeleted)
            {
                item.Show("Delete");

                if (options.DeleteDuplicates)
                {
                    File.Delete(item.Path);
                }
                summary.RegisterFileDeletion(item.FileLength);
            }
        }
    }

    private static void HandleParseError(IEnumerable<Error> errors)
    {
        // TODO: Show errors
    }
}
