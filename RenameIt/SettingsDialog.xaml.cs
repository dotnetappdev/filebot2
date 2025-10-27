using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace RenameIt
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public AppSettings Settings { get; private set; }

        public SettingsDialog()
        {
            this.InitializeComponent();
            Settings = AppSettings.Load();
            LoadSettings();
        }

        private void LoadSettings()
        {
            RecursiveDefaultCheckBox.IsChecked = Settings.RecursiveDefault;
            BackupDefaultCheckBox.IsChecked = Settings.BackupDefault;
            BackupFolderTextBox.Text = Settings.BackupFolder;
            TmdbApiKeyTextBox.Text = Settings.TmdbApiKey;
            TvdbApiKeyTextBox.Text = Settings.TvdbApiKey;
            DefaultFormatTextBox.Text = Settings.DefaultFormatPattern;
            ShowHiddenFilesCheckBox.IsChecked = Settings.ShowHiddenFiles;
            SkipDuplicatesCheckBox.IsChecked = Settings.SkipDuplicates;
            
            ThemeComboBox.SelectedIndex = Settings.Theme switch
            {
                "Light" => 1,
                "Dark" => 2,
                _ => 0
            };
        }

        private void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Settings.RecursiveDefault = RecursiveDefaultCheckBox.IsChecked ?? false;
            Settings.BackupDefault = BackupDefaultCheckBox.IsChecked ?? false;
            Settings.BackupFolder = BackupFolderTextBox.Text;
            Settings.TmdbApiKey = TmdbApiKeyTextBox.Text;
            Settings.TvdbApiKey = TvdbApiKeyTextBox.Text;
            Settings.DefaultFormatPattern = DefaultFormatTextBox.Text;
            Settings.ShowHiddenFiles = ShowHiddenFilesCheckBox.IsChecked ?? false;
            Settings.SkipDuplicates = SkipDuplicatesCheckBox.IsChecked ?? false;
            
            Settings.Theme = ThemeComboBox.SelectedIndex switch
            {
                1 => "Light",
                2 => "Dark",
                _ => "System"
            };
            
            Settings.Save();
        }

        private async void BrowseBackupFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            var hwnd = WindowNative.GetWindowHandle(App.Current.MainWindow);
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                BackupFolderTextBox.Text = folder.Path;
            }
        }

        private void UsePattern_Click(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton button && button.Tag is string pattern)
            {
                DefaultFormatTextBox.Text = pattern;
            }
        }
    }
}
