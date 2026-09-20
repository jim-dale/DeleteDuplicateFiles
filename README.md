# DeleteDuplicateFiles
Identify and optionally delete duplicate files based the SHA256 hash of the file contents

``` Script
Usage:
  DeleteDuplicateFiles [options]

Options:
  -p, --path <path>        Sets the directory to search for duplicate files. [default: The current directory]
  -i, --include <include>  The file patterns to include in the search for duplicates. Separate each pattern with a semicolon. For example '*.jpg;*.png' [default: *]
  -x, --exclude <exclude>  The file patterns to exclude in the search for duplicates. Separate each pattern with a semicolon. For example '*.mp4;*.webm'
  --delete                 Delete the duplicate files. If this option is not specified, the effect of running the command will be shown but the files will not be deleted.
  -s, --summary            Show summary information.
  -v, --verbose            Set output to verbose messages.
  -?, -h, --help           Show help and usage information
  --version                Show version information
```
