using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.FileSystemGlobbing;

namespace DeleteDuplicateFiles
{
    public static class Extensions
    {

        /// <summary>
        /// Group the items by file length
        /// </summary>
        public static IEnumerable<IGrouping<long, FileInfo>> GroupByFileLength(this IEnumerable<FileInfo> items)
        {
            return from fi in items
                   orderby fi.Length
                   group fi by fi.Length into g
                   select g;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<IGrouping<string, FileInfo>> GroupByHash(this IEnumerable<IGrouping<long, FileInfo>> items)
        {
            return from g in items
                   from fi in g
                   let hash = fi.GetHash<MD5CryptoServiceProvider>()
                   group fi by hash into g
                   select g;
        }

        /// <summary>
        /// Filter a group by the size of the group
        /// </summary>
        public static IEnumerable<IGrouping<TKey, TElement>> FilterByGroupSize<TKey, TElement>(this IEnumerable<IGrouping<TKey, TElement>> items, int size)
        {
            return from g in items
                   where g.Count() > size
                   select g;
        }

        public static Matcher AddIncludePatterns(this Matcher result, string patterns, char separator, string defaultValue)
        {
            if (string.IsNullOrEmpty(patterns))
            {
                result.AddInclude(defaultValue);
            }
            else
            {
                var items = patterns.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                result.AddIncludePatterns(items);
            }

            return result;
        }

        public static Matcher AddExcludePatterns(this Matcher result, string patterns, char separator)
        {
            if (string.IsNullOrEmpty(patterns) == false)
            {
                var items = patterns.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                result.AddExcludePatterns(items);
            }

            return result;
        }

        public static string GetHash<T>(this FileInfo item) where T : HashAlgorithm, new()
        {
            string result = default;

            using (var stream = File.OpenRead(item.FullName))
            {
                result = stream.GetHash<T>();
            }

            return result;
        }

        public static string GetHash<T>(this Stream item) where T : HashAlgorithm, new()
        {
            var builder = new StringBuilder();

            using (T algo = new T())
            {
                byte[] hashBytes = algo.ComputeHash(item);

                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
            }

            return builder.ToString();
        }

        public static void Show(this IEnumerable<IGrouping<long, FileInfo>> items)
        {
            var count = items.Count();

            if (count > 0)
            {
                Console.WriteLine($"Groups by file size\\Count={count}");
                foreach (var item in items)
                {
                    item.Show();
                }
                Console.WriteLine();
            }
        }

        public static void Show(this IEnumerable<IGrouping<string, FileInfo>> items)
        {
            var count = items.Count();

            if (count > 0)
            {
                Console.WriteLine($"Groups by hash\\Count={count}");
                foreach (var item in items)
                {
                    item.Show();
                }
                Console.WriteLine();
            }
        }

        public static void Show(this IEnumerable<FileInfo> items)
        {
            var count = items.Count();

            if (count > 0)
            {
                Console.WriteLine($"Files found={count}");
                foreach (var item in items)
                {
                    item.Show();
                }
                Console.WriteLine();
            }
        }

        public static void Show(this FileInfo item, string prefix = null)
        {
            var builder = new StringBuilder("  ");

            if (prefix != null)
            {
                builder.Append(prefix + "\\");
            };
            builder.AppendFormat("Length={0}\\Created='{1}'\\Path=\"{2}\"", item.Length, item.CreationTimeUtc, item.FullName);

            Console.WriteLine(builder);
        }
    }
}
