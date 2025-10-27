using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace RenameIt
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<FileItem> _originalFiles = new();
        private ObservableCollection<RenamedFileItem> _renamedFiles = new();
        private string _currentFormatPattern = "{n} - {s00e00} - {t}";
        private string _currentSource = "TheMovieDB";
        private AppSettings _settings;

        public MainWindow()
        {
            this.InitializeComponent();
            _settings = AppSettings.Load();
            OriginalFilesDataGrid.ItemsSource = _originalFiles;
            RenamedFilesDataGrid.ItemsSource = _renamedFiles;
            
            // Apply settings
            RecursiveCheckBox.IsChecked = _settings.RecursiveDefault;
            BackupCheckBox.IsChecked = _settings.BackupDefault;
            FormatPatternTextBox.Text = _settings.DefaultFormatPattern;
            _currentFormatPattern = _settings.DefaultFormatPattern;
            
            ApplyTheme(_settings.Theme);
        }

        private void ApplyTheme(string theme)
        {
            var requestedTheme = theme switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };

            if (Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = requestedTheme;
            }
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog
            {
                XamlRoot = this.Content.XamlRoot
            };
            
            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                _settings = dialog.Settings;
                RecursiveCheckBox.IsChecked = _settings.RecursiveDefault;
                BackupCheckBox.IsChecked = _settings.BackupDefault;
                FormatPatternTextBox.Text = _settings.DefaultFormatPattern;
                ApplyTheme(_settings.Theme);
            }
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = (Content as FrameworkElement)?.RequestedTheme ?? ElementTheme.Default;
            var newTheme = currentTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
            
            if (Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = newTheme;
            }
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                folderPicker.FileTypeFilter.Add("*");

                var hwnd = WindowNative.GetWindowHandle(this);
                InitializeWithWindow.Initialize(folderPicker, hwnd);

                var folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    await LoadFilesFromFolder(folder, RecursiveCheckBox.IsChecked ?? false);
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error selecting folder: {ex.Message}";
            }
        }

        private async void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePicker = new FileOpenPicker();
                filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                filePicker.FileTypeFilter.Add("*");

                var hwnd = WindowNative.GetWindowHandle(this);
                InitializeWithWindow.Initialize(filePicker, hwnd);

                var files = await filePicker.PickMultipleFilesAsync();
                if (files != null && files.Count > 0)
                {
                    await LoadSelectedFiles(files);
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error selecting files: {ex.Message}";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _originalFiles.Clear();
            _renamedFiles.Clear();
            StatusTextBlock.Text = "Files cleared. Ready.";
            FileCountTextBlock.Text = "";
        }

        private async void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_originalFiles.Count == 0)
            {
                StatusTextBlock.Text = "No files selected";
                return;
            }

            try
            {
                // Show progress bar
                OperationProgressBar.Visibility = Visibility.Visible;
                OperationProgressBar.Value = 0;
                OperationProgressBar.Maximum = _renamedFiles.Count;

                int successCount = 0;
                int errorCount = 0;
                bool shouldBackup = BackupCheckBox.IsChecked ?? false;

                for (int i = 0; i < _renamedFiles.Count; i++)
                {
                    var renamedItem = _renamedFiles[i];
                    try
                    {
                        var originalItem = _originalFiles.FirstOrDefault(f => 
                            Path.Combine(f.DirectoryPath, f.FileName) == 
                            Path.Combine(renamedItem.DirectoryPath, renamedItem.OriginalFileName));

                        if (originalItem != null && !string.IsNullOrEmpty(renamedItem.NewFileName))
                        {
                            string oldPath = Path.Combine(originalItem.DirectoryPath, originalItem.FileName);
                            string newPath = Path.Combine(renamedItem.DirectoryPath, renamedItem.NewFileName);

                            if (File.Exists(oldPath) && oldPath != newPath)
                            {
                                // Backup if requested
                                if (shouldBackup && !string.IsNullOrEmpty(_settings.BackupFolder))
                                {
                                    await BackupFile(oldPath);
                                }

                                File.Move(oldPath, newPath);
                                renamedItem.Status = "✓ Success";
                                successCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        renamedItem.Status = $"✗ Error: {ex.Message}";
                        errorCount++;
                    }

                    // Update progress
                    OperationProgressBar.Value = i + 1;
                    StatusTextBlock.Text = $"Processing {i + 1} of {_renamedFiles.Count}...";
                    await Task.Delay(10); // Allow UI to update
                }

                StatusTextBlock.Text = $"✓ Renamed {successCount} files. {(errorCount > 0 ? $"✗ {errorCount} errors." : "")}";
                
                // Hide progress bar
                OperationProgressBar.Visibility = Visibility.Collapsed;
                
                // Refresh the file list
                if (successCount > 0)
                {
                    await Task.Delay(1000);
                    _originalFiles.Clear();
                    _renamedFiles.Clear();
                    FileCountTextBlock.Text = "";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error during rename: {ex.Message}";
                OperationProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async Task BackupFile(string filePath)
        {
            try
            {
                if (!Directory.Exists(_settings.BackupFolder))
                {
                    Directory.CreateDirectory(_settings.BackupFolder);
                }

                string fileName = Path.GetFileName(filePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupPath = Path.Combine(_settings.BackupFolder, $"{timestamp}_{fileName}");
                
                File.Copy(filePath, backupPath);
                await Task.CompletedTask;
            }
            catch
            {
                // Silently fail backup - don't prevent rename
            }
        }

        private void FormatPatternTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentFormatPattern = FormatPatternTextBox.Text;
            UpdateRenamedPreview();
        }

        private void SourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceComboBox.SelectedItem is ComboBoxItem item)
            {
                _currentSource = item.Content?.ToString() ?? "TheMovieDB";
                UpdateRenamedPreview();
            }
        }

        private async Task LoadFilesFromFolder(StorageFolder folder, bool recursive)
        {
            _originalFiles.Clear();
            _renamedFiles.Clear();

            StatusTextBlock.Text = "Loading files...";
            OperationProgressBar.Visibility = Visibility.Visible;
            OperationProgressBar.IsIndeterminate = true;

            try
            {
                var files = new List<StorageFile>();
                await CollectFilesRecursive(folder, files, recursive);

                foreach (var file in files)
                {
                    var fileItem = new FileItem
                    {
                        FileName = file.Name,
                        DirectoryPath = file.Path.Replace(file.Name, "").TrimEnd('\\'),
                        FileSize = await GetFileSizeString(file)
                    };
                    _originalFiles.Add(fileItem);
                }

                UpdateRenamedPreview();
                StatusTextBlock.Text = $"✓ Loaded {_originalFiles.Count} files from folder";
                FileCountTextBlock.Text = $"{_originalFiles.Count} file(s)";
            }
            finally
            {
                OperationProgressBar.Visibility = Visibility.Collapsed;
                OperationProgressBar.IsIndeterminate = false;
            }
        }

        private async Task CollectFilesRecursive(StorageFolder folder, List<StorageFile> files, bool recursive)
        {
            var folderFiles = await folder.GetFilesAsync();
            files.AddRange(folderFiles);

            if (recursive)
            {
                var subFolders = await folder.GetFoldersAsync();
                foreach (var subFolder in subFolders)
                {
                    await CollectFilesRecursive(subFolder, files, recursive);
                }
            }
        }

        private async Task LoadSelectedFiles(IReadOnlyList<StorageFile> files)
        {
            _originalFiles.Clear();
            _renamedFiles.Clear();

            foreach (var file in files)
            {
                var fileItem = new FileItem
                {
                    FileName = file.Name,
                    DirectoryPath = file.Path.Replace(file.Name, "").TrimEnd('\\'),
                    FileSize = await GetFileSizeString(file)
                };
                _originalFiles.Add(fileItem);
            }

            UpdateRenamedPreview();
            StatusTextBlock.Text = $"✓ Loaded {_originalFiles.Count} files";
            FileCountTextBlock.Text = $"{_originalFiles.Count} file(s)";
        }

        private async Task<string> GetFileSizeString(StorageFile file)
        {
            try
            {
                var properties = await file.GetBasicPropertiesAsync();
                double size = properties.Size;
                string[] sizes = { "B", "KB", "MB", "GB" };
                int order = 0;
                while (size >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    size = size / 1024;
                }
                return $"{size:0.##} {sizes[order]}";
            }
            catch
            {
                return "N/A";
            }
        }

        private void UpdateRenamedPreview()
        {
            _renamedFiles.Clear();

            var parser = new FileNameParser();
            var renamer = new FileRenamer(_currentSource);

            foreach (var file in _originalFiles)
            {
                try
                {
                    var metadata = parser.ParseFileName(file.FileName);
                    var newFileName = renamer.ApplyFormat(_currentFormatPattern, metadata, file.FileName);

                    var renamedItem = new RenamedFileItem
                    {
                        OriginalFileName = file.FileName,
                        NewFileName = newFileName,
                        DirectoryPath = file.DirectoryPath,
                        Status = "Ready"
                    };
                    _renamedFiles.Add(renamedItem);
                }
                catch (Exception ex)
                {
                    var renamedItem = new RenamedFileItem
                    {
                        OriginalFileName = file.FileName,
                        NewFileName = file.FileName,
                        DirectoryPath = file.DirectoryPath,
                        Status = $"Parse Error: {ex.Message}"
                    };
                    _renamedFiles.Add(renamedItem);
                }
            }
        }
    }

    public class FileItem
    {
        public string FileName { get; set; } = string.Empty;
        public string DirectoryPath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
    }

    public class RenamedFileItem
    {
        public string OriginalFileName { get; set; } = string.Empty;
        public string NewFileName { get; set; } = string.Empty;
        public string DirectoryPath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
