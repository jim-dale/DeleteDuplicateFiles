namespace DeleteDuplicateFiles.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeleteDuplicateFiles.Helpers;
using DeleteDuplicateFiles.Models;
using Microsoft.Extensions.FileSystemGlobbing;

public static class Extensions
{
    public static Matcher AddIncludePatterns(this Matcher result, string patterns, char separator, string defaultValue)
    {
        if (string.IsNullOrEmpty(patterns))
        {
            result.AddInclude(defaultValue);
        }
        else
        {
            string[] items = patterns.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            result.AddIncludePatterns(items);
        }

        return result;
    }

    public static Matcher AddExcludePatterns(this Matcher result, string patterns, char separator)
    {
        if (string.IsNullOrEmpty(patterns) == false)
        {
            string[] items = patterns.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            result.AddExcludePatterns(items);
        }

        return result;
    }

    public static void Show(this IEnumerable<IGrouping<string, DedupFileInfo>> items)
    {
        int count = items.Count();

        if (count > 0)
        {
            Console.WriteLine($"Groups by hash\\Count={count}");
            foreach (IGrouping<string, DedupFileInfo> item in items)
            {
                item.Show();
            }
            Console.WriteLine();
        }
    }

    public static void Show(this IEnumerable<DedupFileInfo> items)
    {
        int count = items.Count();

        if (count > 0)
        {
            Console.WriteLine($"Files found={count}");
            foreach (DedupFileInfo item in items)
            {
                item.Show();
            }
            Console.WriteLine();
        }
    }

    public static void Show(this DedupFileInfo item, string? prefix = null)
    {
        StringBuilder builder = new("  ");

        if (prefix != null)
        {
            builder.Append(prefix + "\\");
        };
        builder.AppendFormat("Length={0}\\Created='{1}'\\Path=\"{2}\"", item.FileLength, item.CreationTimeUtc, item.Path);

        Console.WriteLine(builder);
    }
}
