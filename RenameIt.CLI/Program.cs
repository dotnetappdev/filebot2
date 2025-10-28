using Terminal.Gui;
using RenameIt;
using RenameIt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameIt.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if -gui argument is provided
            if (args.Length > 0 && args[0].Equals("-gui", StringComparison.OrdinalIgnoreCase))
            {
                Application.Init();
                try
                {
                    Application.Run<RenameItApp>();
                }
                finally
                {
                    Application.Shutdown();
                }
            }
            else
            {
                Console.WriteLine("RenameIt - File Renaming Tool");
                Console.WriteLine("Usage:");
                Console.WriteLine("  RenameIt.CLI -gui    Launch the GUI console interface");
                Console.WriteLine();
                Console.WriteLine("For command line usage, use the Windows application.");
            }
        }
    }

    public class RenameItApp : Window
    {
        private ListView _filesListView = null!;
        private ListView _previewListView = null!;
        private TextField _formatPatternField = null!;
        private TextField _pathField = null!;
        private ComboBox _sourceComboBox = null!;
        private ComboBox _templateComboBox = null!;
        private CheckBox _recursiveCheckBox = null!;
        private CheckBox _backupCheckBox = null!;
        private Label _statusLabel = null!;
        private Button _renameButton = null!;

        private List<FileItem> _files = new List<FileItem>();
        private List<string> _filePaths = new List<string>();
        private TemplateRepository? _templateRepository;
        private List<RenameTemplate> _templates = new List<RenameTemplate>();
        private string[] _sources = { "TheMovieDB", "TheTVDB", "TVMaze" };
        private string _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public RenameItApp()
        {
            InitializeTemplates();
            InitializeUI();
        }

        private void InitializeTemplates()
        {
            try
            {
                var appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RenameIt");
                var dbPath = Path.Combine(appDataPath, "templates.db");
                _templateRepository = new TemplateRepository(dbPath);
                _templateRepository.SeedDefaultTemplates();
                _templates = _templateRepository.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Template Error", $"Failed to load templates: {ex.Message}", "OK");
                _templates = new List<RenameTemplate>();
            }
        }

        private void InitializeUI()
        {
            // Main window
            Title = "RenameIt - Console GUI (XTree Gold Style)";

            // Create menu bar
            var menu = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem("_File", new MenuItem[]
                {
                    new MenuItem("_Select Folder", "", () => SelectFolder()),
                    new MenuItem("_Select Files", "", () => SelectFiles()),
                    null!,
                    new MenuItem("_Quit", "", () => Application.RequestStop())
                }),
                new MenuBarItem("_Tools", new MenuItem[]
                {
                    new MenuItem("_Templates", "", () => ManageTemplates()),
                    new MenuItem("_Settings", "", () => OpenSettings())
                }),
                new MenuBarItem("_Help", new MenuItem[]
                {
                    new MenuItem("_About", "", () => ShowAbout())
                })
            });

            Add(menu);

            // Path field
            var pathLabel = new Label("Path:")
            {
                X = 0,
                Y = 1
            };
            Add(pathLabel);

            _pathField = new TextField(_currentPath)
            {
                X = Pos.Right(pathLabel) + 1,
                Y = 1,
                Width = Dim.Fill() - 20
            };
            Add(_pathField);

            var browseButton = new Button("Browse")
            {
                X = Pos.Right(_pathField) + 1,
                Y = 1
            };
            browseButton.Clicked += () => SelectFolder();
            Add(browseButton);

            // Options frame
            var optionsFrame = new FrameView("Options")
            {
                X = 0,
                Y = 3,
                Width = Dim.Fill(),
                Height = 5
            };

            _recursiveCheckBox = new CheckBox("Include Subdirectories")
            {
                X = 1,
                Y = 0
            };
            optionsFrame.Add(_recursiveCheckBox);

            _backupCheckBox = new CheckBox("Backup Before Rename")
            {
                X = 30,
                Y = 0,
                Checked = true
            };
            optionsFrame.Add(_backupCheckBox);

            var templateLabel = new Label("Template:")
            {
                X = 1,
                Y = 1
            };
            optionsFrame.Add(templateLabel);

            _templateComboBox = new ComboBox()
            {
                X = Pos.Right(templateLabel) + 1,
                Y = 1,
                Width = 30,
                Height = 5
            };
            _templateComboBox.SetSource(GetTemplateNames());
            _templateComboBox.SelectedItemChanged += OnTemplateSelected;
            optionsFrame.Add(_templateComboBox);

            var formatLabel = new Label("Format:")
            {
                X = 1,
                Y = 2
            };
            optionsFrame.Add(formatLabel);

            _formatPatternField = new TextField("{n} - {s00e00} - {t}")
            {
                X = Pos.Right(formatLabel) + 1,
                Y = 2,
                Width = 40
            };
            optionsFrame.Add(_formatPatternField);

            var sourceLabel = new Label("Source:")
            {
                X = Pos.Right(_formatPatternField) + 2,
                Y = 2
            };
            optionsFrame.Add(sourceLabel);

            _sourceComboBox = new ComboBox()
            {
                X = Pos.Right(sourceLabel) + 1,
                Y = 2,
                Width = 15,
                Height = 4
            };
            _sourceComboBox.SetSource(_sources.ToList());
            _sourceComboBox.SelectedItem = 0;
            optionsFrame.Add(_sourceComboBox);

            Add(optionsFrame);

            // Files panel (left)
            var filesFrame = new FrameView("Original Files")
            {
                X = 0,
                Y = Pos.Bottom(optionsFrame),
                Width = Dim.Percent(50),
                Height = Dim.Fill() - 3
            };

            _filesListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            filesFrame.Add(_filesListView);
            Add(filesFrame);

            // Preview panel (right)
            var previewFrame = new FrameView("Preview (Renamed)")
            {
                X = Pos.Right(filesFrame),
                Y = Pos.Bottom(optionsFrame),
                Width = Dim.Fill(),
                Height = Dim.Fill() - 3
            };

            _previewListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            previewFrame.Add(_previewListView);
            Add(previewFrame);

            // Status bar
            _statusLabel = new Label("Ready. Select files to begin.")
            {
                X = 0,
                Y = Pos.AnchorEnd(2),
                Width = Dim.Fill()
            };
            Add(_statusLabel);

            // Action buttons
            var loadButton = new Button("Load Files")
            {
                X = 0,
                Y = Pos.AnchorEnd(1)
            };
            loadButton.Clicked += () => LoadFiles();
            Add(loadButton);

            var previewButton = new Button("Update Preview")
            {
                X = Pos.Right(loadButton) + 2,
                Y = Pos.AnchorEnd(1)
            };
            previewButton.Clicked += () => UpdatePreview();
            Add(previewButton);

            _renameButton = new Button("Rename Files")
            {
                X = Pos.Right(previewButton) + 2,
                Y = Pos.AnchorEnd(1),
                Enabled = false
            };
            _renameButton.Clicked += () => RenameFiles();
            Add(_renameButton);

            var clearButton = new Button("Clear")
            {
                X = Pos.Right(_renameButton) + 2,
                Y = Pos.AnchorEnd(1)
            };
            clearButton.Clicked += () => ClearFiles();
            Add(clearButton);

            var quitButton = new Button("Quit")
            {
                X = Pos.AnchorEnd(10),
                Y = Pos.AnchorEnd(1)
            };
            quitButton.Clicked += () => Application.RequestStop();
            Add(quitButton);
        }

        private List<string> GetTemplateNames()
        {
            var names = new List<string> { "(None)" };
            names.AddRange(_templates.Select(t => t.Name));
            return names;
        }

        private void OnTemplateSelected(ListViewItemEventArgs args)
        {
            if (args.Item > 0 && args.Item <= _templates.Count)
            {
                var template = _templates[args.Item - 1];
                _formatPatternField.Text = template.Pattern;
            }
        }

        private void SelectFolder()
        {
            var dialog = new OpenDialog("Select Folder", "Select a folder containing files to rename")
            {
                CanChooseDirectories = true,
                CanChooseFiles = false,
                AllowsMultipleSelection = false,
                DirectoryPath = _currentPath
            };

            Application.Run(dialog);

            if (!dialog.Canceled && dialog.FilePaths.Count > 0)
            {
                _currentPath = dialog.FilePaths[0];
                _pathField.Text = _currentPath;
                LoadFiles();
            }
        }

        private void SelectFiles()
        {
            var dialog = new OpenDialog("Select Files", "Select files to rename")
            {
                CanChooseDirectories = false,
                CanChooseFiles = true,
                AllowsMultipleSelection = true,
                DirectoryPath = _currentPath
            };

            Application.Run(dialog);

            if (!dialog.Canceled && dialog.FilePaths.Count > 0)
            {
                _filePaths = dialog.FilePaths.ToList();
                LoadSelectedFiles();
            }
        }

        private void LoadFiles()
        {
            try
            {
                var path = _pathField.Text?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                {
                    MessageBox.ErrorQuery("Error", "Please enter a valid directory path.", "OK");
                    return;
                }

                _files.Clear();
                var searchOption = _recursiveCheckBox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var extensions = new[] { ".mkv", ".mp4", ".avi", ".mov", ".wmv", ".m4v" };
                
                var files = Directory.GetFiles(path, "*.*", searchOption)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .ToList();

                foreach (var file in files)
                {
                    _files.Add(new FileItem
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file),
                        Directory = Path.GetDirectoryName(file) ?? string.Empty
                    });
                }

                UpdateFilesList();
                UpdatePreview();
                _statusLabel.Text = $"Loaded {_files.Count} files.";
                _renameButton.Enabled = _files.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to load files: {ex.Message}", "OK");
            }
        }

        private void LoadSelectedFiles()
        {
            try
            {
                _files.Clear();

                foreach (var filePath in _filePaths)
                {
                    if (File.Exists(filePath))
                    {
                        _files.Add(new FileItem
                        {
                            FilePath = filePath,
                            FileName = Path.GetFileName(filePath),
                            Directory = Path.GetDirectoryName(filePath) ?? string.Empty
                        });
                    }
                }

                UpdateFilesList();
                UpdatePreview();
                _statusLabel.Text = $"Loaded {_files.Count} files.";
                _renameButton.Enabled = _files.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to load files: {ex.Message}", "OK");
            }
        }

        private void UpdateFilesList()
        {
            _filesListView.SetSource(_files.Select(f => f.FileName).ToList());
        }

        private void UpdatePreview()
        {
            try
            {
                var pattern = _formatPatternField.Text?.ToString() ?? string.Empty;
                var source = _sourceComboBox.SelectedItem >= 0 && _sourceComboBox.SelectedItem < _sources.Length
                    ? _sources[_sourceComboBox.SelectedItem]
                    : "TheMovieDB";

                var parser = new FileNameParser();
                var renamer = new FileRenamer(source);
                var previewList = new List<string>();

                foreach (var file in _files)
                {
                    var metadata = parser.ParseFileName(file.FileName);
                    var newName = renamer.ApplyFormat(pattern, metadata, file.FileName);
                    file.NewFileName = newName;
                    previewList.Add(newName);
                }

                _previewListView.SetSource(previewList);
                _statusLabel.Text = $"Preview updated for {_files.Count} files.";
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to update preview: {ex.Message}", "OK");
            }
        }

        private void RenameFiles()
        {
            if (_files.Count == 0)
            {
                MessageBox.ErrorQuery("Error", "No files to rename.", "OK");
                return;
            }

            var result = MessageBox.Query("Confirm Rename", 
                $"Rename {_files.Count} files?", "Yes", "No");

            if (result == 0) // Yes
            {
                try
                {
                    var backupPath = "";
                    if (_backupCheckBox.Checked)
                    {
                        backupPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "RenameIt_Backup",
                            DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                        Directory.CreateDirectory(backupPath);
                    }

                    int renamed = 0;
                    foreach (var file in _files)
                    {
                        if (File.Exists(file.FilePath) && !string.IsNullOrEmpty(file.NewFileName))
                        {
                            var newPath = Path.Combine(file.Directory, file.NewFileName);
                            
                            // Backup if enabled
                            if (_backupCheckBox.Checked)
                            {
                                var backupFile = Path.Combine(backupPath, file.FileName);
                                File.Copy(file.FilePath, backupFile, true);
                            }

                            // Rename
                            File.Move(file.FilePath, newPath, false);
                            renamed++;
                        }
                    }

                    MessageBox.Query("Success", $"Successfully renamed {renamed} files.", "OK");
                    ClearFiles();
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to rename files: {ex.Message}", "OK");
                }
            }
        }

        private void ClearFiles()
        {
            _files.Clear();
            _filePaths.Clear();
            UpdateFilesList();
            _previewListView.SetSource(new List<string>());
            _statusLabel.Text = "Ready. Select files to begin.";
            _renameButton.Enabled = false;
        }

        private void ManageTemplates()
        {
            MessageBox.Query("Templates", "Template management - Feature in development", "OK");
        }

        private void OpenSettings()
        {
            MessageBox.Query("Settings", "Settings dialog - Feature in development", "OK");
        }

        private void ShowAbout()
        {
            MessageBox.Query("About RenameIt",
                "RenameIt - Console GUI\n\n" +
                "A powerful file renaming tool with FileBot-compatible syntax.\n\n" +
                "Built with Terminal.Gui by Miguel de Icaza\n" +
                "Version 1.0.0", "OK");
        }

        private class FileItem
        {
            public string FilePath { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;
            public string Directory { get; set; } = string.Empty;
            public string NewFileName { get; set; } = string.Empty;
        }
﻿using CommandLine;
using RenameIt.CLI;
using Serilog;

// Configure logging
LoggingConfig.ConfigureLogging();

try
{
    // Configure parser to show help when no arguments provided
    var parser = new Parser(with => {
        with.HelpWriter = Console.Out;
        with.AutoHelp = true;
        with.AutoVersion = true;
    });

    // If no arguments provided, show help
    if (args.Length == 0)
    {
        Console.WriteLine("RenameIt CLI - Media File Renaming Tool");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("  renameit <command> [options]");
        Console.WriteLine();
        Console.WriteLine("COMMANDS:");
        Console.WriteLine("  rename      Rename files using a format pattern");
        Console.WriteLine("  preview     Preview renamed files without applying changes");
        Console.WriteLine("  batch       Execute a batch script file with multiple operations");
        Console.WriteLine();
        Console.WriteLine("OPTIONS:");
        Console.WriteLine("  --help      Display help for a specific command");
        Console.WriteLine("  --version   Display version information");
        Console.WriteLine();
        Console.WriteLine("EXAMPLES:");
        Console.WriteLine("  renameit rename \"/media/tv\" \"{n} - {s00e00} - {t}\" -s TheMovieDB");
        Console.WriteLine("  renameit preview \"/media/movies\" \"{n} ({y})\" -r");
        Console.WriteLine("  renameit batch organize-media.txt --dry-run");
        Console.WriteLine();
        Console.WriteLine("For more information on a specific command, use:");
        Console.WriteLine("  renameit <command> --help");
        Console.WriteLine();
        return 0;
    }

    Log.Information("Command line arguments: {Args}", string.Join(" ", args));

    return await parser.ParseArguments<RenameOptions, PreviewOptions, BatchOptions>(args)
        .MapResult(
            (RenameOptions opts) => CommandHandlers.RenameAsync(opts),
            (PreviewOptions opts) => CommandHandlers.PreviewAsync(opts),
            (BatchOptions opts) => CommandHandlers.BatchAsync(opts),
            errs => Task.FromResult(1));
}
finally
{
    LoggingConfig.CloseLogging();
}

namespace RenameIt.CLI
{
    [Verb("rename", HelpText = "Rename files using a format pattern")]
    public class RenameOptions
    {
        [Value(0, Required = true, MetaName = "input", HelpText = "Input file or directory path")]
        public string Input { get; set; } = string.Empty;

        [Value(1, Required = true, MetaName = "pattern", HelpText = "Format pattern (e.g., '{n} - {s00e00} - {t}')")]
        public string Pattern { get; set; } = string.Empty;

        [Option('s', "source", Default = "TheMovieDB", HelpText = "Metadata source: TheMovieDB, TheTVDB, or TVMaze")]
        public string Source { get; set; } = "TheMovieDB";

        [Option('r', "recursive", Default = false, HelpText = "Process subdirectories recursively")]
        public bool Recursive { get; set; }

        [Option('b', "backup", Default = false, HelpText = "Create backup before renaming")]
        public bool Backup { get; set; }

        [Option('d', "dry-run", Default = false, HelpText = "Preview changes without renaming")]
        public bool DryRun { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output directory (default: rename in place)")]
        public string? OutputDirectory { get; set; }

        [Option('f', "filter", Required = false, HelpText = "File extension filter (e.g., 'mkv,mp4,avi')")]
        public string? FileFilter { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }

        [Option('q', "quiet", Default = false, HelpText = "Suppress all output except errors")]
        public bool Quiet { get; set; }

        [Option("skip-existing", Default = false, HelpText = "Skip files if target already exists")]
        public bool SkipExisting { get; set; }

        [Option("overwrite", Default = false, HelpText = "Overwrite existing files")]
        public bool Overwrite { get; set; }

        [Option("max-depth", Default = -1, HelpText = "Maximum directory depth for recursive operations (-1 = unlimited)")]
        public int MaxDepth { get; set; }

        [Option("api-key", Required = false, HelpText = "API key for metadata provider")]
        public string? ApiKey { get; set; }

        [Option("language", Default = "en", HelpText = "Language code for metadata (e.g., 'en', 'de', 'fr')")]
        public string Language { get; set; } = "en";
    }

    [Verb("preview", HelpText = "Preview renamed files without applying changes")]
    public class PreviewOptions
    {
        [Value(0, Required = true, MetaName = "input", HelpText = "Input file or directory path")]
        public string Input { get; set; } = string.Empty;

        [Value(1, Required = true, MetaName = "pattern", HelpText = "Format pattern")]
        public string Pattern { get; set; } = string.Empty;

        [Option('s', "source", Default = "TheMovieDB", HelpText = "Metadata source")]
        public string Source { get; set; } = "TheMovieDB";

        [Option('r', "recursive", Default = false, HelpText = "Process subdirectories recursively")]
        public bool Recursive { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output directory (default: rename in place)")]
        public string? OutputDirectory { get; set; }

        [Option('f', "filter", Required = false, HelpText = "File extension filter (e.g., 'mkv,mp4,avi')")]
        public string? FileFilter { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }

        [Option("max-depth", Default = -1, HelpText = "Maximum directory depth for recursive operations (-1 = unlimited)")]
        public int MaxDepth { get; set; }

        [Option("api-key", Required = false, HelpText = "API key for metadata provider")]
        public string? ApiKey { get; set; }

        [Option("language", Default = "en", HelpText = "Language code for metadata (e.g., 'en', 'de', 'fr')")]
        public string Language { get; set; } = "en";
    }

    [Verb("batch", HelpText = "Execute a batch script file")]
    public class BatchOptions
    {
        [Value(0, Required = true, MetaName = "script", HelpText = "Path to batch script file")]
        public string Script { get; set; } = string.Empty;

        [Option('d', "dry-run", Default = false, HelpText = "Preview changes without renaming")]
        public bool DryRun { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }

        [Option('q', "quiet", Default = false, HelpText = "Suppress all output except errors")]
        public bool Quiet { get; set; }

        [Option("continue-on-error", Default = false, HelpText = "Continue processing if a command fails")]
        public bool ContinueOnError { get; set; }
    }
}
