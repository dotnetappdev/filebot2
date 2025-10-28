# RenameIt.CLI Examples

## Basic Usage

### Launch the Console GUI

```bash
# Using dotnet run
dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj -- -gui

# Or after building
./RenameIt.CLI/bin/Debug/net8.0/RenameIt.CLI -gui
```

### Without GUI (shows help)

```bash
dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj
```

Output:
```
RenameIt - File Renaming Tool
Usage:
  RenameIt.CLI -gui    Launch the GUI console interface

For command line usage, use the Windows application.
```

## GUI Interface Overview

When launched with `-gui`, the application displays:

```
┌─ File ─ Tools ─ Help ────────────────────────────────────────────────────┐
│                                                                            │
│ Path: /home/user/videos                              [Browse]             │
│                                                                            │
│ ┌─ Options ──────────────────────────────────────────────────────────┐   │
│ │ ☑ Include Subdirectories        ☑ Backup Before Rename            │   │
│ │ Template: [TV Show - Standard  ▼]                                  │   │
│ │ Format: {n} - {s00e00} - {t}    Source: [TheMovieDB ▼]            │   │
│ └────────────────────────────────────────────────────────────────────┘   │
│                                                                            │
│ ┌─ Original Files ─────────┐ ┌─ Preview (Renamed) ────────────────┐     │
│ │ Breaking.Bad.S01E01...    │ │ Breaking Bad - S01E01 - Pilot.mkv │     │
│ │ Breaking.Bad.S01E02...    │ │ Breaking Bad - S01E02 - Cat's...  │     │
│ │ The.Matrix.1999...        │ │ The Matrix (1999).mkv             │     │
│ │                           │ │                                    │     │
│ └───────────────────────────┘ └────────────────────────────────────┘     │
│                                                                            │
│ Ready. Select files to begin.                                             │
│ [Load Files] [Update Preview] [Rename Files] [Clear]            [Quit]    │
└────────────────────────────────────────────────────────────────────────────┘
```

## Common Workflows

### Workflow 1: Rename TV Show Episodes

1. Launch the GUI: `RenameIt.CLI -gui`
2. Click or Tab to the Path field
3. Enter the path to your TV show episodes
4. Select "TV Show - Standard" template (or keep default)
5. Click "Load Files"
6. Review the preview pane
7. Click "Rename Files"
8. Confirm the rename operation

### Workflow 2: Rename Movies

1. Launch the GUI: `RenameIt.CLI -gui`
2. Navigate to File > Select Folder
3. Choose your movies folder
4. Select "Movie - Standard" template from dropdown
5. Format pattern changes to: `{n} ({y})`
6. Click "Update Preview"
7. Verify the preview
8. Click "Rename Files"

### Workflow 3: Custom Format Pattern

1. Launch the GUI
2. Load files from a folder
3. Click in the Format field
4. Enter custom pattern: `{n} Season {s} Episode {e} - {t}`
5. Select metadata source (TheMovieDB, TheTVDB, or TVMaze)
6. Click "Update Preview"
7. Review and rename

## Format Patterns

### TV Show Patterns

| Pattern | Example Output |
|---------|----------------|
| `{n} - {s00e00} - {t}` | Breaking Bad - S01E02 - Cat's in the Bag.mkv |
| `{n} {sxe} {t}` | Breaking Bad 1x02 Cat's in the Bag.mkv |
| `{n}/Season {s00}/{n} - {s00e00} - {t}` | Breaking Bad/Season 01/Breaking Bad - S01E02 - Cat's in the Bag.mkv |

### Movie Patterns

| Pattern | Example Output |
|---------|----------------|
| `{n} ({y})` | The Matrix (1999).mkv |
| `{n} - {y}` | The Matrix - 1999.mkv |
| `{fn}` | The.Matrix.1999.BluRay.mkv (original filename) |

### Available Tokens

- `{n}` - Show or movie name
- `{s}` - Season number
- `{e}` - Episode number
- `{s00}` - Season with leading zero (01)
- `{e00}` - Episode with leading zero (02)
- `{s00e00}` - Combined (S01E02)
- `{sxe}` - Combined alternate (1x02)
- `{t}` - Episode title
- `{y}` - Year
- `{ext}` - File extension
- `{fn}` - Original filename
- `{source}` - Metadata source

## Mouse Operations

- **Click**: Select items, activate buttons
- **Double-click**: Edit text fields
- **Drag**: Scroll through lists
- **Right-click**: Context menu (where available)

## Keyboard Shortcuts

- **Tab**: Navigate between controls
- **Shift+Tab**: Navigate backwards
- **Enter**: Activate focused button
- **Escape**: Cancel dialogs
- **Alt+F**: File menu
- **Alt+T**: Tools menu
- **Alt+H**: Help menu
- **Arrow keys**: Navigate lists and dropdowns

## Tips

1. **Enable Backup**: Always check "Backup Before Rename" for safety
2. **Preview First**: Always use "Update Preview" before renaming
3. **Templates**: Save frequently used patterns as templates
4. **Recursive Scan**: Enable "Include Subdirectories" for folder hierarchies
5. **Test Small**: Test with a few files before batch renaming

## Troubleshooting

### Application doesn't start
- Ensure .NET 8.0 SDK is installed
- Check terminal supports ANSI escape sequences
- Try running: `dotnet --version`

### Files not loading
- Verify the path exists
- Check file extensions (.mkv, .mp4, .avi, etc.)
- Ensure read permissions on the directory

### Preview shows wrong names
- Verify the format pattern is correct
- Check the selected metadata source
- Ensure files follow standard naming conventions

## Advanced Features

### Template Management
- Access via Tools > Templates menu
- Create, edit, and delete custom templates
- Templates are stored in SQLite database
- Shared between Windows GUI and Console GUI

### Settings
- Access via Tools > Settings menu
- Configure API keys for metadata sources
- Set default backup folder
- Customize default format pattern

## Examples with Sample Files

Given these files:
```
Breaking.Bad.S01E01.Pilot.mkv
Breaking.Bad.S01E02.Cats.in.the.Bag.mkv
The.Office.US.S02E01.720p.mkv
Inception.2010.1080p.BluRay.mkv
```

Using pattern: `{n} - {s00e00} - {t}`

Results:
```
Breaking Bad - S01E01 - Pilot.mkv
Breaking Bad - S01E02 - Cat's in the Bag.mkv
The Office US - S02E01 - The Dundies.mkv
Inception (2010).mkv
```

## Integration with Existing Workflow

The Console GUI:
- Uses the same RenameIt.Core library as Windows GUI
- Shares template database with Windows application
- Provides identical renaming logic
- Cross-platform (Windows, Linux, macOS)

This ensures consistent behavior regardless of interface used.
