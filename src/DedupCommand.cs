namespace DeleteDuplicateFiles;

using System;
using System.Collections.Generic;
using DeleteDuplicateFiles.Helpers;
using DeleteDuplicateFiles.Models;

internal class DedupCommand
{
    private readonly DedupOptions options;
    private readonly FileFinderWithHash fileFinder;

    public DedupCommand(DedupOptions options, FileFinderWithHash fileFinder)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(fileFinder);

        this.options = options;
        this.fileFinder = fileFinder;
    }

    public int Execute()
    {
        Summary summary = new(this.options.DeleteDuplicates == false);

        IEnumerable<DedupFileInfo> items = this.fileFinder.GetItems();

        this.ProcessFiles(items, summary);

        if (this.options.ShowSummary)
        {
            summary.Show();
        }

        return 0;
    }

    internal void ProcessFiles(IEnumerable<DedupFileInfo> items, Summary summary)
    {
        if (this.options.Verbose)
        {
            items.Show();
        }

        IEnumerable<IGrouping<string, DedupFileInfo>> groups = items.GroupBy(x => x.HashString).Where(g => g.Count() > 1);

        if (this.options.Verbose)
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

                if (this.options.DeleteDuplicates)
                {
                    File.Delete(item.Path);
                }
                summary.RegisterFileDeletion(item.FileLength);
            }
        }
    }
}
