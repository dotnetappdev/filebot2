# RenameIt CLI User Guide

## Overview

RenameIt CLI is a command-line tool for batch renaming media files for Plex, Kodi, and other streaming services. It uses the same powerful FileBot-compatible syntax as the GUI version.

## Installation

Build the CLI project:
```bash
dotnet build RenameIt.CLI/RenameIt.CLI.csproj
```

Run directly:
```bash
dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj -- [command] [options]
```

Or publish as a standalone executable:
```bash
dotnet publish RenameIt.CLI/RenameIt.CLI.csproj -c Release -r win-x64 --self-contained
```

## Commands

### rename
Rename files using a format pattern.

```bash
renameit rename <input> <pattern> [options]
```

**Arguments:**
- `input` - Input file or directory path
- `pattern` - Format pattern (e.g., '{n} - {s00e00} - {t}')

**Options:**
- `-s, --source` - Metadata source: TheMovieDB, TheTVDB, or TVMaze (default: TheMovieDB)
- `-r, --recursive` - Process subdirectories recursively (default: false)
- `-b, --backup` - Create backup before renaming (default: false)
- `-d, --dry-run` - Preview changes without renaming (default: false)

**Examples:**

Rename TV show episodes:
```bash
renameit rename "/path/to/tv/shows" "{n} - {s00e00} - {t}" -s TheMovieDB
```

Rename movies with backup:
```bash
renameit rename "/path/to/movies" "{n} ({y})" -s TheMovieDB -b
```

Preview changes without renaming:
```bash
renameit rename "/path/to/files" "{n} - {s00e00}" -d
```

Recursive directory processing:
```bash
renameit rename "/path/to/media" "{n} - {s00e00} - {t}" -r
```

### preview
Preview renamed files without applying changes (equivalent to `rename --dry-run`).

```bash
renameit preview <input> <pattern> [options]
```

**Arguments:**
- `input` - Input file or directory path
- `pattern` - Format pattern

**Options:**
- `-s, --source` - Metadata source (default: TheMovieDB)
- `-r, --recursive` - Process subdirectories recursively (default: false)

**Example:**
```bash
renameit preview "/path/to/shows" "{n} - {s00e00} - {t}"
```

### batch
Execute a batch script file with multiple rename operations.

```bash
renameit batch <script> [options]
```

**Arguments:**
- `script` - Path to batch script file

**Options:**
- `-d, --dry-run` - Preview changes without renaming (default: false)

**Example:**
```bash
renameit batch "/path/to/rename-script.txt"
```

## Batch Script Format

Batch scripts allow you to define multiple rename operations in a single file. Each section starts with a section header in brackets, followed by key-value pairs.

**Example batch script:**
```ini
# This is a comment
# Rename TV shows
[TV Shows]
input=/path/to/tv/shows
pattern={n} - {s00e00} - {t}
source=TheMovieDB
recursive=false
backup=true

# Rename movies
[Movies]
input=/path/to/movies
pattern={n} ({y})
source=TheMovieDB
recursive=false
backup=false

# Organize anime
[Anime]
input=/path/to/anime
pattern={n} - {sxe} - {t}
source=TheTVDB
recursive=true
backup=true
```

**Batch Script Properties:**
- `input` - Directory or file path to process (required)
- `pattern` - Format pattern to use (required)
- `source` - Metadata source (default: TheMovieDB)
- `recursive` - Process subdirectories (default: false)
- `backup` - Create backups before renaming (default: false)

**Comments:**
- Lines starting with `#` or `//` are treated as comments
- Empty lines are ignored

## Format Patterns

RenameIt CLI supports the same FileBot-compatible format syntax as the GUI:

### TV Show Tokens
- `{n}` - Show name (e.g., "Breaking Bad")
- `{s}` - Season number (e.g., "1")
- `{e}` - Episode number (e.g., "2")
- `{s00}` - Season with zero padding (e.g., "01")
- `{e00}` - Episode with zero padding (e.g., "02")
- `{s00e00}` - Season and episode (e.g., "S01E02")
- `{sxe}` - Alternative format (e.g., "1x02")
- `{t}` - Episode title

### Movie Tokens
- `{n}` - Movie name (e.g., "The Matrix")
- `{y}` - Release year (e.g., "1999")

### General Tokens
- `{ext}` - File extension without dot (e.g., "mkv")
- `{source}` - Metadata source (e.g., "TheMovieDB")
- `{fn}` - Original filename without extension

### Common Patterns

**TV Shows:**
- `{n} - {s00e00} - {t}` → "Breaking Bad - S01E02 - Cat's in the Bag.mkv"
- `{n} - Season {s} Episode {e}` → "Friends - Season 1 Episode 2.mkv"
- `{n} {sxe} {t}` → "The Office 1x02 Diversity Day.mkv"

**Movies:**
- `{n} ({y})` → "The Matrix (1999).mkv"
- `{y} - {n}` → "1999 - The Matrix.mkv"
- `{n}` → "Inception.mkv"

## Metadata Sources

RenameIt CLI supports three metadata sources:

1. **TheMovieDB** - Best for movies and popular TV shows
   - Comprehensive database with extensive metadata
   - Good for both TV and movies

2. **TheTVDB** - Traditional TV database
   - Extensive TV series catalog
   - Excellent for older and niche shows

3. **TVMaze** - Current TV show information
   - Good for currently airing shows
   - Up-to-date episode information

## Workflow Examples

### Example 1: Organize a TV Show Collection

```bash
# Preview the changes first
renameit preview "/media/shows/breaking-bad" "{n} - {s00e00} - {t}"

# If satisfied, rename with backup
renameit rename "/media/shows/breaking-bad" "{n} - {s00e00} - {t}" -b
```

### Example 2: Batch Process Multiple Folders

Create a batch script `organize-media.txt`:
```ini
[Breaking Bad]
input=/media/shows/breaking-bad
pattern={n} - {s00e00} - {t}
source=TheMovieDB
backup=true

[Movies]
input=/media/movies
pattern={n} ({y})
source=TheMovieDB
backup=true
```

Execute the script:
```bash
renameit batch organize-media.txt
```

### Example 3: Recursive Processing

Rename all video files in a directory tree:
```bash
renameit rename "/media" "{n} - {s00e00} - {t}" -r -d
```

## Best Practices

1. **Always preview first**: Use `-d` or the `preview` command to check results before renaming
2. **Enable backups**: Use `-b` for important files to create `.backup` copies
3. **Start small**: Test on a small subset before processing large collections
4. **Use batch scripts**: Save commonly used patterns in batch scripts for reuse
5. **Check metadata sources**: Try different sources if one doesn't give good results

## Supported File Extensions

RenameIt CLI automatically detects video files with these extensions:
- `.mkv`, `.mp4`, `.avi`, `.mov`
- `.wmv`, `.flv`, `.m4v`
- `.mpg`, `.mpeg`

## Exit Codes

- `0` - Success
- `1` - Error (file not found, parse error, rename failed, etc.)

## Troubleshooting

### No files found
- Check that the input path exists
- Verify the path uses the correct separators for your OS
- Check file extensions are supported

### Parse errors
- Ensure filenames follow standard naming conventions
- Try using `-d` to see how files are being parsed
- Manually rename problematic files to a standard format first

### Rename failures
- Check file permissions
- Verify target filenames don't already exist
- Use `-b` to create backups before renaming
