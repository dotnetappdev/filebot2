# CLI Implementation Summary

## Overview
This implementation adds a complete command-line interface (CLI) system to RenameIt, providing full parity with the GUI for file renaming operations. The CLI is designed for automation, scripting, and batch processing of media files for Plex, Kodi, and other streaming services.

## What Was Implemented

### 1. RenameIt.CLI Console Project
- **Location**: `RenameIt.CLI/`
- **Framework**: .NET 8.0
- **Package**: CommandLineParser 2.9.1
- **Entry Point**: `Program.cs`
- **Command Handlers**: `CommandHandlers.cs`

### 2. Three Core Commands

#### `rename` Command
Rename files using a format pattern with full control.
```bash
renameit rename <input> <pattern> [options]
```
Options:
- `-s, --source`: Metadata source (TheMovieDB, TheTVDB, TVMaze)
- `-r, --recursive`: Process subdirectories
- `-b, --backup`: Create backups before renaming
- `-d, --dry-run`: Preview without renaming

#### `preview` Command
Preview renamed files without applying changes.
```bash
renameit preview <input> <pattern> [options]
```

#### `batch` Command
Execute batch script files with multiple rename operations.
```bash
renameit batch <script> [--dry-run]
```

### 3. Batch Script System
- **Format**: INI-style configuration
- **Features**: 
  - Comments support (`#` or `//`)
  - Multiple sections per script
  - Per-section configuration
  - All rename options supported

**Example**:
```ini
[TV Shows]
input=/path/to/shows
pattern={n} - {s00e00} - {t}
source=TheMovieDB
recursive=false
backup=true
```

### 4. Architecture
```
RenameIt.Core/          # Business logic (shared by GUI and CLI)
├── FileNameParser.cs   # Parse filenames to extract metadata
├── FileRenamer.cs      # Apply format patterns
└── MetadataProviders.cs # Query metadata APIs

RenameIt.CLI/           # CLI interface
├── Program.cs          # Command definitions and parsing
└── CommandHandlers.cs  # Command implementations

RenameIt/               # GUI (Windows-only)
RenameIt.Tests/         # Unit tests for all components
```

### 5. Testing
- **New Tests**: 8 comprehensive CLI tests
- **Total Tests**: 23 (15 original + 8 CLI)
- **Coverage**:
  - Dry-run functionality
  - Actual file renaming
  - Backup creation
  - Recursive processing
  - Batch script execution
  - Error handling
  - Non-existent paths

All tests pass successfully.

### 6. Documentation

#### CLI_GUIDE.md (6,867 characters)
- Complete command reference
- All options documented
- Format pattern syntax
- Metadata source information
- Best practices
- Troubleshooting

#### CLI_EXAMPLES.md (5,612 characters)
- 8+ real-world examples
- Plex/Kodi integration examples
- Automation scripts
- Cron job setup
- Batch processing workflows

#### example-batch-script.txt (2,583 characters)
- Fully commented example
- Multiple scenarios
- Best practice demonstrations

### 7. Helper Scripts
- **renameit.sh**: Linux/macOS wrapper script
- **renameit.bat**: Windows wrapper script
- Make CLI easier to use without full dotnet commands

## Key Features

### Cross-Platform Support
- Windows: ✅
- Linux: ✅
- macOS: ✅

### FileBot-Compatible Syntax
All format patterns from the GUI are supported:
- `{n}` - Name (show/movie)
- `{s}`, `{e}` - Season/episode numbers
- `{s00e00}` - Formatted season/episode
- `{t}` - Episode title
- `{y}` - Year
- `{ext}`, `{fn}`, `{source}` - File metadata

### Metadata Sources
- TheMovieDB
- TheTVDB
- TVMaze

### Safety Features
- Dry-run mode (preview without changes)
- Backup creation before renaming
- Detailed error messages
- Exit codes for scripting

## Usage Examples

### Simple Rename
```bash
./renameit.sh rename "/media/tv/breaking-bad" "{n} - {s00e00} - {t}"
```

### Preview Before Rename
```bash
./renameit.sh preview "/media/movies" "{n} ({y})"
```

### Batch Processing
```bash
./renameit.sh batch organize-media.txt
```

### With Safety Features
```bash
./renameit.sh rename "/media/tv" "{n} - {s00e00}" -r -b
```

## Testing Summary

### Unit Tests
```
✓ Dry-run does not rename files
✓ Valid input renames files correctly
✓ Backup creates backup files
✓ Non-existent path returns error
✓ Preview returns success without renaming
✓ Batch with non-existent script returns error
✓ Batch with valid script processes commands
✓ Recursive processes subdirectories
```

### Manual Testing
- ✅ CLI help output
- ✅ Command-specific help
- ✅ Preview command with test files
- ✅ Batch script execution
- ✅ Wrapper script functionality

## Security Review

**CodeQL Analysis**: ✅ No security issues found
- No SQL injection risks
- No command injection risks
- No path traversal vulnerabilities
- Proper input validation
- Safe file operations

## Performance Considerations

### Efficiency
- Shared core logic with GUI (no duplication)
- Compiled regex patterns in FileNameParser
- Efficient file filtering by extension
- Minimal memory footprint

### Scalability
- Can process thousands of files
- Recursive directory scanning
- Batch script support for automation

## Integration Opportunities

### Media Servers
- **Plex**: Compatible naming conventions
- **Kodi**: Standard format support
- **Jellyfin**: FileBot syntax compatibility
- **Emby**: Media library organization

### Automation
- Cron jobs (Linux/macOS)
- Task Scheduler (Windows)
- Download completion scripts
- CI/CD pipelines

### Scripting
- Bash scripts
- PowerShell scripts
- Python wrappers
- Batch files

## Future Enhancements (Not in Scope)

### Possible Additions
1. **Terminal.Gui TUI**: Interactive console UI
2. **Config File**: Save default settings
3. **Templates**: Save/load format patterns
4. **Regex Patterns**: Custom filename parsing
5. **Plugin System**: Custom metadata providers
6. **Progress Bars**: Visual feedback for large batches

## Files Added/Modified

### New Files
```
RenameIt.CLI/
├── Program.cs
├── CommandHandlers.cs
└── RenameIt.CLI.csproj

RenameIt.Tests/
└── CLICommandHandlersTests.cs

Documentation/
├── CLI_GUIDE.md
├── CLI_EXAMPLES.md
└── example-batch-script.txt

Scripts/
├── renameit.sh
└── renameit.bat
```

### Modified Files
```
README.md           # Added CLI information
RenameIt.sln        # Added CLI project
RenameIt.Tests.csproj  # Added CLI reference
```

## Success Metrics

- ✅ CLI system fully functional
- ✅ Parity with GUI features
- ✅ Comprehensive testing (23 tests, 100% pass)
- ✅ Complete documentation
- ✅ Cross-platform support
- ✅ Batch script support
- ✅ No security vulnerabilities
- ✅ Zero breaking changes to existing code

## Conclusion

The CLI implementation successfully addresses all requirements from the original issue:

1. ✅ Full command-line features for renaming
2. ✅ Script support with batch file syntax
3. ✅ Can run without GUI
4. ✅ Business logic separated in RenameIt.Core
5. ✅ Console program in separate project
6. ✅ Supports typical renaming arguments
7. ✅ Suitable for Plex, Kodi, and other streaming services
8. ✅ Feature parity with GUI

The implementation is production-ready, well-tested, well-documented, and follows best practices for .NET CLI applications.
