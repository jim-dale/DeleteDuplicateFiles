namespace DeleteDuplicateFiles.Models;

using System;

public record class DedupFileInfo(string Path, long FileLength, int FileNameLength, DateTimeOffset CreationTimeUtc, byte[] Hash, string HashString);
