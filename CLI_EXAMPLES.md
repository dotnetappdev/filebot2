# RenameIt CLI Examples

This file demonstrates practical examples of using the RenameIt CLI tool.

## Basic Examples

### Example 1: Preview TV Show Renaming
```bash
# Preview how files will be renamed without making changes
./renameit.sh preview "/path/to/tv/shows" "{n} - {s00e00} - {t}"
```

### Example 2: Rename TV Show Episodes
```bash
# Rename TV show files with TheMovieDB metadata
./renameit.sh rename "/path/to/breaking-bad" "{n} - {s00e00} - {t}" -s TheMovieDB

# With backup enabled
./renameit.sh rename "/path/to/breaking-bad" "{n} - {s00e00} - {t}" -s TheMovieDB -b
```

### Example 3: Rename Movie Files
```bash
# Rename movies with year in parentheses
./renameit.sh rename "/path/to/movies" "{n} ({y})" -s TheMovieDB -b

# Alternative pattern: Year first
./renameit.sh rename "/path/to/movies" "{y} - {n}" -s TheMovieDB
```

### Example 4: Recursive Directory Processing
```bash
# Process all subdirectories recursively
./renameit.sh rename "/path/to/media" "{n} - {s00e00} - {t}" -r -b

# Dry run with recursive processing
./renameit.sh rename "/path/to/media" "{n} - {s00e00} - {t}" -r -d
```

## Advanced Examples

### Example 5: Different Metadata Sources
```bash
# Use TheTVDB for TV shows
./renameit.sh rename "/path/to/shows" "{n} - {s00e00} - {t}" -s TheTVDB

# Use TVMaze for current shows
./renameit.sh rename "/path/to/shows" "{n} - {s00e00} - {t}" -s TVMaze
```

### Example 6: Alternative Naming Patterns
```bash
# Simple format: Show Name 1x02 Episode Title
./renameit.sh rename "/path/to/shows" "{n} {sxe} {t}" -s TheMovieDB

# Detailed format: Show Name - Season 1 Episode 2
./renameit.sh rename "/path/to/shows" "{n} - Season {s} Episode {e}" -s TheMovieDB

# Movie with source: The Matrix (1999) [TheMovieDB]
./renameit.sh rename "/path/to/movies" "{n} ({y}) [{source}]" -s TheMovieDB
```

## Batch Script Examples

### Example 7: Simple Batch Script
Create a file named `rename-shows.txt`:
```ini
[Breaking Bad]
input=/media/tv/breaking-bad
pattern={n} - {s00e00} - {t}
source=TheMovieDB
backup=true

[Friends]
input=/media/tv/friends
pattern={n} - {s00e00} - {t}
source=TheMovieDB
backup=true
```

Execute:
```bash
./renameit.sh batch rename-shows.txt
```

### Example 8: Complex Batch Script
Create a file named `organize-library.txt`:
```ini
# Organize entire media library
[TV Shows]
input=/media/tv
pattern={n} - {s00e00} - {t}
source=TheMovieDB
recursive=true
backup=true

[Movies]
input=/media/movies
pattern={n} ({y})
source=TheMovieDB
recursive=false
backup=true

[Anime]
input=/media/anime
pattern={n} - {sxe} - {t}
source=TheTVDB
recursive=true
backup=false
```

Execute with dry run first:
```bash
./renameit.sh batch organize-library.txt --dry-run
```

Then execute for real:
```bash
./renameit.sh batch organize-library.txt
```

## Real-World Workflows

### Workflow 1: New TV Show Season
```bash
# 1. Preview the changes
./renameit.sh preview "/downloads/show-season-2" "{n} - {s00e00} - {t}"

# 2. If satisfied, rename with backup
./renameit.sh rename "/downloads/show-season-2" "{n} - {s00e00} - {t}" -b

# 3. Move to permanent location (manual step)
mv /downloads/show-season-2 /media/tv/show-name/
```

### Workflow 2: Organize Downloaded Movies
```bash
# 1. Preview all movies in downloads
./renameit.sh preview "/downloads" "{n} ({y})" -r

# 2. Rename with backup
./renameit.sh rename "/downloads" "{n} ({y})" -b -r

# 3. Move to movies library (manual step)
```

### Workflow 3: Mass Organization with Batch Script
Create `weekly-organization.txt`:
```ini
[New Downloads]
input=/downloads
pattern={n} - {s00e00} - {t}
source=TheMovieDB
recursive=true
backup=true
```

Set up as weekly task:
```bash
# Run every week to organize downloads
./renameit.sh batch weekly-organization.txt
```

## Integration with Plex/Kodi

### Plex Naming Convention
```bash
# Plex prefers: Show Name - s01e02 - Episode Title
./renameit.sh rename "/media/plex/tv" "{n} - {s00e00} - {t}" -r

# Movies: Movie Name (Year)
./renameit.sh rename "/media/plex/movies" "{n} ({y})" -r
```

### Kodi Naming Convention
```bash
# Kodi format: Show Name - s01e02
./renameit.sh rename "/media/kodi/tv" "{n} - {s00e00}" -r

# Movies: Movie Name (Year)
./renameit.sh rename "/media/kodi/movies" "{n} ({y})" -r
```

## Automation Scripts

### Bash Script for Automated Organization
Create `auto-organize.sh`:
```bash
#!/bin/bash

# Auto-organize downloaded media files

DOWNLOADS_DIR="/downloads"
TV_DIR="/media/tv"
MOVIES_DIR="/media/movies"

# Organize TV shows
echo "Organizing TV shows..."
./renameit.sh rename "$DOWNLOADS_DIR/tv" "{n} - {s00e00} - {t}" -r -b

# Organize movies
echo "Organizing movies..."
./renameit.sh rename "$DOWNLOADS_DIR/movies" "{n} ({y})" -r -b

echo "Organization complete!"
```

### Cron Job Setup (Linux)
```bash
# Add to crontab to run daily at 3 AM
0 3 * * * /path/to/renameit.sh batch /path/to/batch-script.txt
```

## Tips and Best Practices

1. **Always Preview First**
   ```bash
   ./renameit.sh preview "/path" "{pattern}" -r
   ```

2. **Use Backups for Important Files**
   ```bash
   ./renameit.sh rename "/path" "{pattern}" -b
   ```

3. **Test on Small Batches**
   ```bash
   # Test on one folder first
   ./renameit.sh rename "/path/to/single-show" "{n} - {s00e00} - {t}" -d
   ```

4. **Keep Batch Scripts**
   - Save your favorite renaming patterns in batch scripts
   - Reuse them for consistent naming across your library

5. **Use Different Metadata Sources**
   - Try different sources if one doesn't give good results
   - TheMovieDB: Best for movies and popular shows
   - TheTVDB: Great for TV series
   - TVMaze: Good for current shows
