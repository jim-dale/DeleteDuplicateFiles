namespace DeleteDuplicateFiles;

using System;
using System.CommandLine;
using DeleteDuplicateFiles.Helpers;
using DeleteDuplicateFiles.Models;

internal static class Program
{
    internal static int Main(string[] args)
    {
        Option<DirectoryInfo> pathOption = new("--path", "-p")
        {
            Description = "Sets the directory to search for duplicate files.",
            DefaultValueFactory = _ => new DirectoryInfo(Directory.GetCurrentDirectory()),
            Arity = ArgumentArity.ZeroOrOne,
        };
        Option<string> includeOption = new("--include", "-i")
        {
            Description = "The file patterns to include in the search for duplicates. Separate each pattern with a semicolon. For example '*.jpg;*.png'",
            DefaultValueFactory = _ => FileFinderWithHash.DefaultIncludePattern,
            Arity = ArgumentArity.ZeroOrOne,
        };
        Option<string> excludeOption = new("--exclude", "-x")
        {
            Description = "The file patterns to exclude in the search for duplicates. Separate each pattern with a semicolon. For example '*.mp4;*.webm'",
            DefaultValueFactory = _ => string.Empty,
            Arity = ArgumentArity.ZeroOrOne,
        };
        Option<bool> deleteOption = new("--delete")
        {
            Description = "Delete the duplicate files. If this option is not specified, the effect of running the command will be shown but the files will not be deleted.",
        };
        Option<bool> summaryOption = new("--summary", "-s")
        {
            Description = "Show summary information.",
        };
        Option<bool> verboseOption = new("--verbose", "-v")
        {
            Description = "Set output to verbose messages.",
        };

        RootCommand rootCommand = new("Delete duplicate files in a directory")
        {
            Options = { pathOption, includeOption, excludeOption, deleteOption, summaryOption, verboseOption, },
        };

        rootCommand.SetAction(parseResult =>
        {
            FileFinderOptions finderOptions = new()
            {
                Path = parseResult.GetRequiredValue(pathOption),
                Include = parseResult.GetRequiredValue(includeOption),
                Exclude = parseResult.GetRequiredValue(excludeOption),
            };
            FileFinderWithHash fileFinder = new(finderOptions);

            DedupOptions dedupOptions = new()
            {
                DeleteDuplicates = parseResult.GetRequiredValue(deleteOption),
                ShowSummary = parseResult.GetRequiredValue(summaryOption),
                Verbose = parseResult.GetRequiredValue(verboseOption),
            };
            DedupCommand command = new(dedupOptions, fileFinder);

            return command.Execute();
        });

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
