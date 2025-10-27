# RenameIt

A Windows file renaming application built with WinUI 3 and C#, inspired by FileBot's powerful renaming capabilities and using FileBot-compatible naming syntax.

## Features

### Core Functionality
- **Dual-Pane Interface**: View original files on the left and renamed preview on the right
- **FileBot-Compatible Syntax**: Use familiar format patterns like `{n} - {s00e00} - {t}` (see [FileBot naming reference](https://www.filebot.net/naming.html))
- **Multiple Metadata Sources**: 
  - [TheMovieDB](https://www.themoviedb.org/?language=en-GB) - Comprehensive movie and TV database
  - [TheTVDB](https://www.thetvdb.com/) - Extensive TV show database
  - TVMaze - Current TV show information
- **Flexible File Selection**: Select files individually or by folder
- **Recursive Directory Scanning**: Optionally scan subdirectories for files
- **Smart Parsing**: Automatically detects TV shows and movies from filenames
- **Live Preview**: See renamed files before applying changes

### Advanced Features
- **Settings Dialog**: Configure API keys, default patterns, backup folder, and more
- **Backup Before Rename**: Automatically backup files before renaming
- **Theme Support**: Light and dark mode with system default option
- **Progress Feedback**: Modern progress bar showing rename operations
- **Keyboard Shortcuts**: Ctrl+O (folder), Ctrl+Shift+O (files), Ctrl+R (rename), Ctrl+, (settings)
- **Visual Feedback**: Icons, status indicators, and file count display

## Format Syntax

RenameIt supports the following FileBot-compatible format patterns:

- `{n}` - Name (show name or movie name)
- `{s}` - Season number
- `{e}` - Episode number
- `{s00}` - Season number (2 digits, e.g., 01)
- `{e00}` - Episode number (2 digits, e.g., 02)
- `{s00e00}` - Season and episode (e.g., S01E02)
- `{sxe}` - Season and episode (e.g., 1x02)
- `{t}` - Episode title
- `{y}` - Year
- `{source}` - Metadata source
- `{ext}` - File extension
- `{fn}` - Original filename without extension

### Example Patterns

- TV Shows: `{n} - {s00e00} - {t}` → "Breaking Bad - S01E02 - Cat's in the Bag"
- Movies: `{n} ({y})` → "The Matrix (1999)"
- Custom: `{n} - Season {s} Episode {e}` → "Friends - Season 1 Episode 2"

## Building

**Note**: This is a Windows-specific application built with WinUI 3. It requires Windows 10/11 to build and run.

### Requirements

- Windows 10 version 1809 (build 17763) or later
- Windows 11
- .NET 8 SDK or later
- Visual Studio 2022 (recommended) with:
  - .NET Desktop Development workload
  - Windows App SDK components

### Build Instructions

1. Open `RenameIt.sln` in Visual Studio 2022
2. Restore NuGet packages
3. Build the solution (F7)
4. Run the application (F5)

Or using the command line:

```bash
dotnet restore
dotnet build
dotnet run
```

## Usage

### Basic Workflow

1. **Select Files**: Click "Select Files" or "Select Folder" to load files
   - Check "Include Subdirectories" to scan folders recursively
2. **Choose Format**: Enter your desired format pattern in the text box
3. **Select Source**: Choose a metadata source from the dropdown
4. **Preview**: The right pane shows how files will be renamed
5. **Rename**: Click "Rename Files" to apply the changes
   - Check "Backup Before Rename" to save copies of original files

### Settings (Ctrl+,)

Access the settings dialog to configure:

- **API Keys**: Enter API keys for TheMovieDB and TheTVDB for enhanced metadata
- **Backup Folder**: Set default location for file backups
- **Default Format Pattern**: Set your preferred naming pattern
- **Theme**: Choose Light, Dark, or System default theme
- **Advanced Options**: Show hidden files, skip duplicates, etc.

### Keyboard Shortcuts

- `Ctrl+O` - Select Folder
- `Ctrl+Shift+O` - Select Files
- `Ctrl+Delete` - Clear Files
- `Ctrl+R` - Rename Files
- `Ctrl+,` - Open Settings
- Theme Toggle Button - Switch between light and dark mode

## Project Structure

```
RenameIt/
├── App.xaml / App.xaml.cs         - Application entry point
├── MainWindow.xaml / .xaml.cs     - Main UI and logic
└── RenameIt.csproj                - Project configuration

RenameIt.Core/
├── FileNameParser.cs              - Parses filenames to extract metadata
├── FileRenamer.cs                 - Applies format patterns to create new names
├── MetadataProviders.cs           - Interfaces with movie/TV databases
└── RenameIt.Core.csproj           - Core library configuration

RenameIt.Tests/
├── FileNameParserTests.cs         - Parser unit tests
├── FileRenamerTests.cs            - Renamer unit tests
└── RenameIt.Tests.csproj          - Test project configuration
```

## Architecture

- **FileNameParser**: Extracts show names, seasons, episodes, and years from filenames
- **FileRenamer**: Applies format patterns using FileBot-compatible syntax
- **MetadataProviders**: Fetch additional metadata from online sources
- **MainWindow**: Manages the UI with dual data grids for original and renamed files

## License

MIT License - See LICENSE file for details
