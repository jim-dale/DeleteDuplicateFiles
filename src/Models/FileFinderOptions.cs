namespace DeleteDuplicateFiles.Models;

using System.Security.Cryptography;

internal class FileFinderOptions
{
    public DirectoryInfo Path { get; set; } = new DirectoryInfo(Directory.GetCurrentDirectory());

    public string Include { get; set; } = string.Empty;

    public string Exclude { get; set; } = string.Empty;

    public string HashAlgorithmName { get; set; } = nameof(SHA256);
}
