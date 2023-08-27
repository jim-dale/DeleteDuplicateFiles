namespace DeleteDuplicateFiles;

using CommandLine;

internal class Options
{
    [Option('p', "path", Required = false, Default = ".", HelpText = "Sets the directory to search for duplicate files.")]
    public string Path { get; set; } = string.Empty;

    [Option('i', "include", Required = false, Default = "*", HelpText = "The file patterns to include in the search for duplicates. Separate each pattern with a semicolon character. For example '*.jpg;*.png'")]
    public string Include { get; set; } = string.Empty;

    [Option('x', "exclude", Required = false, HelpText = "The file patterns to exclude in the search for duplicates. Separate each pattern with a semicolon character. For example '*.mp4;*.webm'")]
    public string Exclude { get; set; } = string.Empty;

    [Option("all", Required = false, HelpText = "Process all files as a single list of files.")]
    public bool All { get; set; }

    [Option("delete", Required = false, Default = false, HelpText = "Delete the duplicate files.")]
    public bool DeleteDuplicates { get; set; }

    [Option('s', "summary", Required = false, HelpText = "Show summary.")]
    public bool ShowSummary { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }
}
