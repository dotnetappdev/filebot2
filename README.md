# RenameIt

A powerful file renaming application inspired by FileBot's renaming capabilities and using FileBot-compatible naming syntax. Available as both a Windows GUI application (WinUI 3) and a cross-platform console GUI (Terminal.Gui).

## Applications

### Windows GUI (RenameIt)
A modern Windows application built with WinUI 3, providing a rich graphical interface with dual-pane layout, template management, and theme support.

### Console GUI (RenameIt.CLI)
A cross-platform console interface built with Terminal.Gui, providing full mouse support and an XTree Gold-style interface. Launch with `-gui` argument.

```bash
RenameIt.CLI -gui
```

See [RenameIt.CLI/README.md](RenameIt.CLI/README.md) for console GUI documentation.

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
- **Cross-Platform Console GUI**: Terminal.Gui interface with full mouse support (RenameIt.CLI)

### Advanced Features
- **Template Management**: Save and reuse rename patterns with built-in template database
- **Settings Dialog**: Configure API keys, default patterns, backup folder, and more
- **Backup Before Rename**: Automatically backup files before renaming
- **Theme Support**: Light and dark mode with system default option (Windows GUI)
- **Progress Feedback**: Modern progress bar showing rename operations
- **Keyboard Shortcuts**: Ctrl+O (folder), Ctrl+Shift+O (files), Ctrl+R (rename), Ctrl+, (settings)
- **Visual Feedback**: Icons, status indicators, and file count display
- **Mouse Support**: Full mouse interaction in console GUI (Terminal.Gui)


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

### Windows GUI (RenameIt)

**Note**: The Windows GUI is a Windows-specific application built with WinUI 3. It requires Windows 10/11 to build and run.

#### Requirements

- Windows 10 version 1809 (build 17763) or later
- Windows 11
- .NET 8 SDK or later
- Visual Studio 2022 (recommended) with:
  - .NET Desktop Development workload
  - Windows App SDK components

#### Build Instructions

1. Open `RenameIt.sln` in Visual Studio 2022
2. Restore NuGet packages
3. Build the solution (F7)
4. Run the application (F5)

Or using the command line:

```bash
dotnet restore
dotnet build RenameIt/RenameIt.csproj
```

### Console GUI (RenameIt.CLI)

**Note**: The Console GUI is cross-platform and works on Windows, Linux, and macOS.

#### Requirements

- .NET 8 SDK or later
- Terminal that supports ANSI/VT100 escape sequences

#### Build Instructions

Using the command line:

```bash
dotnet restore
dotnet build RenameIt.CLI/RenameIt.CLI.csproj
```

#### Run Instructions

```bash
dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj -- -gui
```

Or after building:

```bash
./RenameIt.CLI/bin/Debug/net8.0/RenameIt.CLI -gui
```

## Usage

### Windows GUI

#### Basic Workflow

1. **Select Files**: Click "Select Files" or "Select Folder" to load files
   - Check "Include Subdirectories" to scan folders recursively
2. **Choose Template (Optional)**: Select a saved template from the dropdown or click "Manage Templates"
3. **Choose Format**: Enter your desired format pattern in the text box (or use a template)
4. **Select Source**: Choose a metadata source from the dropdown
5. **Preview**: The right pane shows how files will be renamed
6. **Rename**: Click "Rename Files" to apply the changes
   - Check "Backup Before Rename" to save copies of original files

#### Keyboard Shortcuts

- `Ctrl+O` - Select Folder
- `Ctrl+Shift+O` - Select Files
- `Ctrl+Delete` - Clear Files
- `Ctrl+R` - Rename Files
- `Ctrl+,` - Open Settings
- Theme Toggle Button - Switch between light and dark mode

### Console GUI

#### Basic Workflow

1. **Launch**: Run `RenameIt.CLI -gui` to start the console interface
2. **Navigate**: Use mouse clicks or Tab key to navigate between controls
3. **Select Files**: 
   - Enter a path and click "Load Files", or
   - Use File > Select Folder/Files from the menu
4. **Configure Options**:
   - Select a template from the dropdown (optional)
   - Enter or modify the format pattern
   - Choose a metadata source
   - Enable/disable recursive scanning and backup options
5. **Preview**: Click "Update Preview" to see how files will be renamed
6. **Rename**: Click "Rename Files" to apply changes

#### Mouse Support

The console GUI provides full mouse support:
- Click buttons to execute actions
- Click in lists to select items
- Click and drag to scroll
- Click menu items to access features
- Click text fields to edit

### Template Management

Save commonly used rename patterns for quick reuse:

1. **Using Templates**: 
   - Select a template from the "Template" dropdown to auto-fill the pattern
   - Templates include pre-configured patterns for TV shows, movies, and more
   
2. **Managing Templates**:
   - Click "Manage Templates" to open the template manager
   - **Add**: Create new templates with custom patterns
   - **Edit**: Modify existing templates
   - **Delete**: Remove templates you no longer need
   
3. **Default Templates**: The app includes 5 built-in templates:
   - TV Show - Standard: `{n} - {s00e00} - {t}`
   - TV Show - Compact: `{n} {sxe} {t}`
   - Movie - Standard: `{n} ({y})`
   - TV Show - Plex: `{n}/Season {s00}/{n} - {s00e00} - {t}`
   - TV Show - Custom Season: `{n} - Season {s} Episode {e}`

Templates are stored in a local SQLite database and persist across sessions. Templates are shared between the Windows GUI and Console GUI.

### Settings

Access the settings dialog to configure:

- **API Keys**: Enter API keys for TheMovieDB and TheTVDB for enhanced metadata
- **Backup Folder**: Set default location for file backups
- **Default Format Pattern**: Set your preferred naming pattern
- **Theme**: Choose Light, Dark, or System default theme (Windows GUI only)
- **Advanced Options**: Show hidden files, skip duplicates, etc.

## Project Structure

```
RenameIt/
├── App.xaml / App.xaml.cs                   - Application entry point
├── MainWindow.xaml / .xaml.cs               - Main UI and logic
├── SettingsDialog.xaml / .xaml.cs           - Settings configuration UI
├── TemplatesDialog.xaml / .xaml.cs          - Template management UI
├── TemplateEditDialog.xaml / .xaml.cs       - Template add/edit UI
├── AppSettings.cs                           - Application settings persistence
└── RenameIt.csproj                          - Project configuration

RenameIt.CLI/
├── Program.cs                                - Console GUI application with Terminal.Gui
├── README.md                                 - Console GUI documentation
└── RenameIt.CLI.csproj                       - CLI project configuration

RenameIt.Core/
├── FileNameParser.cs                        - Parses filenames to extract metadata
├── FileRenamer.cs                           - Applies format patterns to create new names
├── MetadataProviders.cs                     - Interfaces with movie/TV databases
├── RenameTemplate.cs                        - Template data model
├── TemplateRepository.cs                    - SQLite database operations for templates
└── RenameIt.Core.csproj                     - Core library configuration

RenameIt.Tests/
├── FileNameParserTests.cs                   - Parser unit tests
├── FileRenamerTests.cs                      - Renamer unit tests
├── TemplateRepositoryTests.cs               - Template repository unit tests
└── RenameIt.Tests.csproj                    - Test project configuration
```

## Architecture

- **RenameIt.Core**: Shared library containing all core functionality
  - Used by both Windows GUI and Console GUI
  - Ensures consistent behavior across platforms
- **FileNameParser**: Extracts show names, seasons, episodes, and years from filenames
- **FileRenamer**: Applies format patterns using FileBot-compatible syntax
- **MetadataProviders**: Fetch additional metadata from online sources
- **TemplateRepository**: Manages template CRUD operations with SQLite database
- **MainWindow**: Manages the UI with dual data grids for original and renamed files
- **TemplatesDialog**: Provides UI for managing saved rename templates

## License

MIT License - See LICENSE file for details
