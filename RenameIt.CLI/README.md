# RenameIt.CLI - Console GUI

A modern console-based GUI for RenameIt, built with Terminal.Gui by Miguel de Icaza. This provides a full-featured, mouse-enabled terminal interface inspired by classic file managers like XTree Gold.

## Features

- **Modern Console GUI**: Terminal.Gui interface with full mouse support
- **Dual-Pane Layout**: View original files and preview renamed files side-by-side
- **XTree Gold Style**: Classic file manager aesthetics in a modern console application
- **Full Feature Parity**: Access all RenameIt functionality from the console
- **Template Support**: Use and manage rename templates
- **Multiple Metadata Sources**: TheMovieDB, TheTVDB, and TVMaze support
- **Live Preview**: See renamed files before applying changes
- **Backup Support**: Automatically backup files before renaming
- **FileBot-Compatible Syntax**: Use the same format patterns as FileBot

## Usage

Launch the console GUI interface:

```bash
dotnet run --project RenameIt.CLI -- -gui
```

Or after building:

```bash
RenameIt.CLI -gui
```

## Interface

The console GUI provides:

1. **Menu Bar**: Access File, Tools, and Help menus
2. **Path Field**: Enter or browse to select a folder
3. **Options Panel**: 
   - Template selection dropdown
   - Format pattern input
   - Metadata source selector
   - Recursive directory scanning option
   - Backup before rename option
4. **Dual Panes**:
   - Left: Original files
   - Right: Preview of renamed files
5. **Action Buttons**: Load Files, Update Preview, Rename Files, Clear, Quit
6. **Status Bar**: Real-time status updates

## Keyboard Shortcuts

- **Alt+F**: File menu
- **Alt+T**: Tools menu
- **Alt+H**: Help menu
- **Tab**: Navigate between controls
- **Enter**: Activate buttons
- **Arrow Keys**: Navigate lists and combo boxes

## Mouse Support

Full mouse support is available:
- Click to select items
- Click buttons to execute actions
- Click and drag to scroll lists
- Click menu items to access features

## Format Syntax

Supports FileBot-compatible format patterns:

- `{n}` - Name (show name or movie name)
- `{s00e00}` - Season and episode (e.g., S01E02)
- `{t}` - Episode title
- `{y}` - Year
- `{ext}` - File extension
- And more...

See the main README for complete syntax documentation.

## Shared Code

The CLI interface shares the same core functionality as the Windows application through the `RenameIt.Core` library, ensuring consistent behavior across all platforms.

## Requirements

- .NET 8.0 or later
- Terminal that supports ANSI/VT100 escape sequences
- Works on Windows, Linux, and macOS

## Building

```bash
dotnet build RenameIt.CLI/RenameIt.CLI.csproj
```

## Running

```bash
dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj -- -gui
```
