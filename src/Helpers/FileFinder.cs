namespace DeleteDuplicateFiles.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DeleteDuplicateFiles.Models;
using Microsoft.Extensions.FileSystemGlobbing;

internal class FileFinder
{
    private readonly Options options;
    private Matcher? matcher;

    public FileFinder(Options options)
    {
        this.options = options;
    }

    public IEnumerable<DedupFileInfo> GetItems()
    {
        this.matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
                .AddIncludePatterns(this.options.Include, ';', "*")
                .AddExcludePatterns(this.options.Exclude, ';');

        IEnumerable<string> files = this.matcher.GetResultsInFullPath(this.options.Path);

        foreach (string file in files)
        {
            FileInfo fileInfo = new(file);

            byte[] hash = ComputeMd5HashForFile(fileInfo.FullName);
            string hashString = BitConverter.ToString(hash);

            yield return new DedupFileInfo(fileInfo.FullName, fileInfo.Length, fileInfo.Name.Length, fileInfo.CreationTimeUtc, hash, hashString);
        }
    }

    private static byte[] ComputeMd5HashForFile(string path)
    {
        using HashAlgorithm algorithm = MD5.Create();
        using FileStream stream = File.OpenRead(path);

        return algorithm.ComputeHash(stream);
    }
}
