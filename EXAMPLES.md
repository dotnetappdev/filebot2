# Example Workflows

This document provides real-world examples of how to use FileBot2 for common file renaming tasks.

## Workflow 1: Organizing a TV Show Collection

### Scenario
You've downloaded a season of Breaking Bad with poorly named files:
- `brba_101.mkv`
- `brba_102.mkv`
- `brba_103.mkv`
- etc.

### Steps

1. **Load Files**
   - Click "Select Folder"
   - Navigate to your Breaking Bad folder
   - Select the folder

2. **Configure Format**
   - Pattern: `{n} - {s00e00} - {t}`
   - Source: TheMovieDB

3. **Preview Results**
   Left Pane (Original):
   ```
   brba_101.mkv
   brba_102.mkv
   brba_103.mkv
   ```
   
   Right Pane (Renamed):
   ```
   Breaking Bad - S01E01 - Pilot.mkv
   Breaking Bad - S01E02 - Cat's in the Bag.mkv
   Breaking Bad - S01E03 - ...and the Bag's in the River.mkv
   ```

4. **Apply Renaming**
   - Review the preview
   - Click "Rename Files"
   - Files are renamed on disk

### Result
Clean, organized filenames that are easy to browse and work with media servers.

---

## Workflow 2: Movie Collection with Year

### Scenario
Downloaded movies with release info in the filename:
- `the.matrix.1999.1080p.bluray.x264.mkv`
- `inception.2010.720p.web-dl.mkv`
- `interstellar.2014.2160p.uhd.mkv`

### Steps

1. **Load Files**
   - Click "Select Files"
   - Select all movie files

2. **Configure Format**
   - Pattern: `{n} ({y})`
   - Source: TheMovieDB

3. **Preview Results**
   Left Pane:
   ```
   the.matrix.1999.1080p.bluray.x264.mkv
   inception.2010.720p.web-dl.mkv
   interstellar.2014.2160p.uhd.mkv
   ```
   
   Right Pane:
   ```
   The Matrix (1999).mkv
   Inception (2010).mkv
   Interstellar (2014).mkv
   ```

4. **Apply Renaming**
   - Click "Rename Files"

### Result
Clean movie filenames with just the title and year.

---

## Workflow 3: Batch Processing Multiple Shows

### Scenario
You have folders for different shows that all need renaming:
```
TV Shows/
  ├── The Office/
  │   ├── the.office.us.s02e01.mkv
  │   └── the.office.us.s02e02.mkv
  ├── Friends/
  │   ├── Friends - 1x01.avi
  │   └── Friends - 1x02.avi
  └── Game of Thrones/
      ├── got.s01e01.mkv
      └── got.s01e02.mkv
```

### Steps

Process each show folder separately:

**For The Office:**
1. Select folder: `TV Shows/The Office/`
2. Pattern: `{n} - {s00e00} - {t}`
3. Result: `The Office - S02E01 - The Dundies.mkv`

**For Friends:**
1. Select folder: `TV Shows/Friends/`
2. Pattern: `{n} - {s00e00} - {t}`
3. Result: `Friends - S01E01 - The One Where Monica Gets a Roommate.mkv`

**For Game of Thrones:**
1. Select folder: `TV Shows/Game of Thrones/`
2. Pattern: `{n} - {s00e00} - {t}`
3. Result: `Game of Thrones - S01E01 - Winter Is Coming.mkv`

---

## Workflow 4: Custom Pattern with Folders

### Scenario
You want to organize files into season folders with a specific naming pattern.

### Steps

1. **Current State**
   ```
   Breaking Bad/
     ├── breaking.bad.s01e01.mkv
     ├── breaking.bad.s01e02.mkv
     ├── breaking.bad.s02e01.mkv
     └── breaking.bad.s02e02.mkv
   ```

2. **Note**: Currently FileBot2 renames files in-place. For folder organization:
   - Manually create season folders first
   - Move files to appropriate folders
   - Then use FileBot2 to rename

3. **Future Enhancement**
   - Pattern like: `{n}/Season {s00}/{n} - {s00e00} - {t}`
   - Would automatically organize into folders

---

## Workflow 5: Handling Special Characters

### Scenario
Episode titles have special characters that need cleaning:
- `show.s01e01.pilot-part.1.mkv` → Title: "Pilot: Part 1"

### Steps

1. Load the file
2. Use standard pattern: `{n} - {s00e00} - {t}`
3. FileBot2 automatically cleans invalid characters:
   - `:` becomes ` -`
   - `/` is removed
   - `?` is removed

Result: `Show - S01E01 - Pilot - Part 1.mkv`

---

## Workflow 6: Verifying Before Renaming

### Scenario
You want to be sure the renaming will work correctly before applying.

### Best Practices

1. **Start Small**
   - Test with a few files first
   - Verify the pattern works

2. **Check the Preview**
   - Review each renamed file in the right pane
   - Look for parse errors in the Status column

3. **Common Issues to Check**
   - Episode numbers parsed correctly?
   - Show names properly capitalized?
   - Special characters handled?
   - File extensions preserved?

4. **Use Test Folder**
   - Copy a few files to a test folder
   - Rename there first
   - Once confirmed, process the full collection

---

## Workflow 7: Mixed Content (Movies and TV)

### Scenario
A folder contains both movies and TV episodes.

### Steps

**Process in two passes:**

1. **First Pass - TV Shows**
   - Select folder
   - Pattern: `{n} - {s00e00} - {t}`
   - Review preview - TV shows will parse correctly
   - Movies will show parse errors
   - Rename only the TV shows (or clear movies first)

2. **Second Pass - Movies**
   - Clear the list
   - Select only movie files
   - Pattern: `{n} ({y})`
   - Rename movies

---

## Tips and Tricks

### Tip 1: Save Common Patterns
While FileBot2 doesn't have pattern saving yet, keep your favorite patterns in a text file:
```
TV Shows: {n} - {s00e00} - {t}
Movies: {n} ({y})
Anime: {n} - {sxe} - {t}
Documentaries: {n} ({y}) - {t}
```

### Tip 2: Backup First
Before batch renaming:
1. Make a backup of your files
2. Or test in a copy of the folder
3. Once confirmed working, apply to originals

### Tip 3: Check File Extensions
FileBot2 preserves file extensions automatically, so:
- `.mkv` stays `.mkv`
- `.avi` stays `.avi`
- `.mp4` stays `.mp4`

### Tip 4: Metadata Source Selection
Different sources may have different data:
- **TheMovieDB**: Best for movies and popular TV shows
- **TVMaze**: Good for TV shows, especially current ones
- **TheTVDB**: Traditional TV database, extensive catalog

Try switching sources if one doesn't give good results.

### Tip 5: Handling Parse Failures
If a file shows "Parse Error":
1. Check the filename format
2. Try renaming it manually to a standard format first
3. Or use a simpler pattern temporarily
