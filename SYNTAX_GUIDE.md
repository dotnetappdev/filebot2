# FileBot2 Format Syntax Guide

This guide provides comprehensive examples of the format syntax used in FileBot2 for renaming files.

## Basic Format Tokens

### TV Show Tokens

| Token | Description | Example Output |
|-------|-------------|----------------|
| `{n}` | Show name | "Breaking Bad" |
| `{s}` | Season number | "1" |
| `{e}` | Episode number | "2" |
| `{s00}` | Season with padding | "01" |
| `{e00}` | Episode with padding | "02" |
| `{s00e00}` | Season and episode | "S01E02" |
| `{sxe}` | Alternative format | "1x02" |
| `{t}` | Episode title | "Pilot" |

### Movie Tokens

| Token | Description | Example Output |
|-------|-------------|----------------|
| `{n}` | Movie name | "The Matrix" |
| `{y}` | Release year | "1999" |

### General Tokens

| Token | Description | Example Output |
|-------|-------------|----------------|
| `{ext}` | File extension | "mkv" |
| `{source}` | Metadata source | "TheMovieDB" |

## Example Renaming Patterns

### TV Shows

#### Pattern 1: Standard Format
**Pattern**: `{n} - {s00e00} - {t}`

**Input**: `breaking.bad.s01e02.mkv`  
**Output**: `Breaking Bad - S01E02 - Cat's in the Bag.mkv`

#### Pattern 2: Alternate Format
**Pattern**: `{n} - Season {s} Episode {e} - {t}`

**Input**: `breaking.bad.s01e02.mkv`  
**Output**: `Breaking Bad - Season 1 Episode 2 - Cat's in the Bag.mkv`

#### Pattern 3: Simple Format
**Pattern**: `{n} {sxe} {t}`

**Input**: `breaking.bad.s01e02.mkv`  
**Output**: `Breaking Bad 1x02 Cat's in the Bag.mkv`

#### Pattern 4: With Folders
**Pattern**: `{n}/Season {s00}/{n} - {s00e00} - {t}`

**Input**: `breaking.bad.s01e02.mkv`  
**Output**: `Breaking Bad/Season 01/Breaking Bad - S01E02 - Cat's in the Bag.mkv`

### Movies

#### Pattern 1: Name and Year
**Pattern**: `{n} ({y})`

**Input**: `the.matrix.1999.1080p.mkv`  
**Output**: `The Matrix (1999).mkv`

#### Pattern 2: Year First
**Pattern**: `{y} - {n}`

**Input**: `the.matrix.1999.1080p.mkv`  
**Output**: `1999 - The Matrix.mkv`

## Supported Input Formats

FileBot2 can parse the following common filename patterns:

### TV Show Formats

1. `Show.Name.S01E02.Title.mkv`
2. `Show.Name.s01e02.title.mkv`
3. `Show Name - 1x02 - Title.mkv`
4. `Show Name 1x02 Title.mkv`
5. `Show_Name_S01E02_Title.mkv`
6. `show.name.102.mkv` (season 1, episode 2)

### Movie Formats

1. `Movie.Name.2020.1080p.BluRay.mkv`
2. `Movie Name (2020).mkv`
3. `Movie_Name_2020_720p.mkv`
4. `Movie.Name.2020.mkv`

## Tips

1. **Test First**: Always preview the renamed files before applying changes
2. **Use Consistent Patterns**: Stick to one format pattern for your entire collection
3. **Check for Special Characters**: FileBot2 automatically removes invalid filename characters
4. **Backup Important Files**: Make backups before batch renaming
5. **Process in Batches**: Rename one folder at a time to catch any issues early

## Advanced Usage

### Conditional Formatting

While basic conditional formatting is not yet implemented, you can work around it by:
- Processing movies and TV shows separately
- Using different patterns for different file types
- Manually adjusting the pattern for specific needs

### Metadata Sources

FileBot2 supports three metadata sources:
1. **TheMovieDB**: Best for movies and TV shows
2. **TVMaze**: Good for TV show metadata
3. **TheTVDB**: Traditional TV database

Choose the source that best matches your content type.
