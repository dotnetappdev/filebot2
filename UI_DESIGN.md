# RenameIt UI Design

This document describes the user interface layout and design of RenameIt.

## Main Window Layout

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ RenameIt - File Renaming Tool                                      [_][□][X]│
├─────────────────────────────────────────────────────────────────────────────┤
│ [Select Folder] [Select Files] [Clear]                   [Rename Files]     │
├─────────────────────────────────────────────────────────────────────────────┤
│ Format Pattern: [  {n} - {s00e00} - {t}  ] [TheMovieDB ▼]                  │
├───────────────────────────────────┬─────────────────────────────────────────┤
│ Original Files                    │ Renamed Files Preview                   │
├───────────────────────────────────┼─────────────────────────────────────────┤
│ File Name            Path    Size │ New File Name        Path       Status  │
├───────────────────────────────────┼─────────────────────────────────────────┤
│ breaking.bad.s01e02  C:\TV  25 MB │ Breaking Bad - S01E… C:\TV      Ready   │
│ the.office.s02e05    C:\TV  22 MB │ The Office - S02E05… C:\TV      Ready   │
│ friends.1x02         C:\TV  20 MB │ Friends - S01E02 -…  C:\TV      Ready   │
│ matrix.1999.1080p    C:\Movies    │ The Matrix (1999)    C:\Movies  Ready   │
│ inception.2010       C:\Movies    │ Inception (2010)     C:\Movies  Ready   │
│                                   │                                         │
│                                   │                                         │
│                                   │                                         │
│                                   │                                         │
│                                   │                                         │
│                                   │                                         │
│                                   │                                         │
│                                   │                                         │
├───────────────────────────────────┴─────────────────────────────────────────┤
│ Ready                                                                       │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Components

### Top Menu Bar
- **Select Folder**: Opens folder picker to load all files from a directory
- **Select Files**: Opens file picker to select specific files
- **Clear**: Clears all loaded files
- **Rename Files**: Applies the renaming to all files (accent button style)

### Format Pattern Section
- **Format Pattern TextBox**: Editable text box with placeholder showing example pattern
  - Pre-filled with `{n} - {s00e00} - {t}` as default
  - Updates preview in real-time as user types
- **Source ComboBox**: Dropdown to select metadata source
  - Options: TheMovieDB, TVMaze, TheTVDB
  - Default: TheMovieDB

### Dual-Pane Data Grid Layout

#### Left Pane: Original Files
- **DataGrid** showing:
  - **File Name**: Original filename
  - **Path**: Directory path
  - **Size**: File size (formatted: KB, MB, GB)
- Read-only grid
- Alternating row backgrounds for readability
- Horizontal grid lines

#### Right Pane: Renamed Files Preview
- **DataGrid** showing:
  - **New File Name**: Renamed filename based on pattern
  - **Path**: Directory path (same as original)
  - **Status**: "Ready", "Success", or error message
- Read-only grid
- Alternating row backgrounds for readability
- Horizontal grid lines
- Columns synchronized with left pane

### Status Bar
- **TextBlock**: Shows current status messages
  - Examples:
    - "Ready"
    - "Loaded 15 files from folder"
    - "Renamed 15 files. 0 errors."
    - "Error: No files selected"

## Color Scheme

Uses Windows 11 default theme:
- Background: System background colors
- Grid alternating rows: Light gray
- Accent button: System accent color
- Borders: Medium gray
- Status bar: Slightly darker than main background

## Interactions

### File Selection Flow
1. User clicks "Select Folder" or "Select Files"
2. Windows picker dialog appears
3. User selects folder/files
4. Files populate in left grid
5. Preview automatically generates in right grid
6. Status bar shows "Loaded X files"

### Preview Updates
- Preview updates automatically when:
  - Format pattern changes (text input)
  - Metadata source changes (dropdown)
  - New files are loaded

### Renaming Flow
1. User reviews preview in right pane
2. User clicks "Rename Files" button
3. Files are renamed on disk
4. Status column updates to "Success" or error
5. Status bar shows summary
6. Grids clear automatically if all successful

## Responsive Behavior

- Window minimum size: 800x600
- Data grids auto-size columns to fit content
- Long filenames are truncated with ellipsis (…)
- Tooltip shows full filename on hover
- Split between left/right panes is 50/50 fixed

## Accessibility

- All buttons have keyboard shortcuts (Alt+ accelerators)
- Tab order flows logically through controls
- High contrast mode supported
- Screen reader compatible with proper ARIA labels
- Focus indicators visible on all interactive elements

## Error States

### Parse Error in Preview
- Status column shows: "Parse Error: [message]"
- New filename falls back to original filename

### Rename Error
- Status column shows: "Error: [message]"
- Examples:
  - "Error: File in use"
  - "Error: Access denied"
  - "Error: Invalid filename"

### Empty State
When no files loaded:
- Both grids show empty
- Status bar shows "Ready"
- Rename button is enabled but shows message if clicked
