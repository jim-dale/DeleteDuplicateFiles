namespace DeleteDuplicateFiles.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DeleteDuplicateFiles.Models;
using Microsoft.Extensions.FileSystemGlobbing;

internal class FileFinderWithHash
{
    private readonly FileFinderOptions options;
    private Matcher? matcher;

    public const string DefaultIncludePattern = "*";
    public const char DefaultPatternSeparator = ';';

    public FileFinderWithHash(FileFinderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.options = options;
    }

    public IEnumerable<DedupFileInfo> GetItems()
    {
        // Select algorithm explicitly (no obsolete API)
        using HashAlgorithm hashAlgorithm = this.options.HashAlgorithmName.ToUpperInvariant() switch
        {
            "SHA256" => SHA256.Create(),
            "SHA1" => SHA1.Create(),
            "MD5" => MD5.Create(),
            _ => throw new ArgumentException($"Unsupported hash algorithm: {this.options.HashAlgorithmName}")
        };

        this.matcher = new Matcher(StringComparison.OrdinalIgnoreCase)
                .AddIncludePatternsOrDefault(this.options.Include, DefaultIncludePattern, DefaultPatternSeparator)
                .AddExcludePatterns(this.options.Exclude, DefaultPatternSeparator);

        IEnumerable<string> files = this.matcher.GetResultsInFullPath(this.options.Path.FullName);

        foreach (string file in files)
        {
            FileInfo fileInfo = new(file);

            using var stream = File.OpenRead(file);
            byte[] hash = hashAlgorithm.ComputeHash(stream);
            string hashString = Convert.ToHexStringLower(hash);

            yield return new DedupFileInfo(fileInfo.FullName, fileInfo.Length, fileInfo.Name.Length, fileInfo.CreationTimeUtc, hash, hashString);
        }
    }
}
