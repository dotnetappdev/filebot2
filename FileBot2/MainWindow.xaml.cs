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

namespace FileBot2
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<FileItem> _originalFiles = new();
        private ObservableCollection<RenamedFileItem> _renamedFiles = new();
        private string _currentFormatPattern = "{n} - {s00e00} - {t}";
        private string _currentSource = "TheMovieDB";

        public MainWindow()
        {
            this.InitializeComponent();
            OriginalFilesDataGrid.ItemsSource = _originalFiles;
            RenamedFilesDataGrid.ItemsSource = _renamedFiles;
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                folderPicker.FileTypeFilter.Add("*");

                // Get the window handle for the picker
                var hwnd = WindowNative.GetWindowHandle(this);
                InitializeWithWindow.Initialize(folderPicker, hwnd);

                var folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    await LoadFilesFromFolder(folder);
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

                // Get the window handle for the picker
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
                int successCount = 0;
                int errorCount = 0;

                foreach (var renamedItem in _renamedFiles)
                {
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
                                File.Move(oldPath, newPath);
                                renamedItem.Status = "Success";
                                successCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        renamedItem.Status = $"Error: {ex.Message}";
                        errorCount++;
                    }
                }

                StatusTextBlock.Text = $"Renamed {successCount} files. {errorCount} errors.";
                
                // Refresh the file list
                if (successCount > 0)
                {
                    _originalFiles.Clear();
                    _renamedFiles.Clear();
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error during rename: {ex.Message}";
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

        private async Task LoadFilesFromFolder(StorageFolder folder)
        {
            _originalFiles.Clear();
            _renamedFiles.Clear();

            var files = await folder.GetFilesAsync();
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
            StatusTextBlock.Text = $"Loaded {_originalFiles.Count} files from folder";
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
            StatusTextBlock.Text = $"Loaded {_originalFiles.Count} files";
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
