namespace DeleteDuplicateFiles.Models;

using System;
using System.IO;

public record class DedupFileInfo(string Path, long FileLength, int FileNameLength, DateTimeOffset CreationTimeUtc, byte[] Hash, string HashString);
