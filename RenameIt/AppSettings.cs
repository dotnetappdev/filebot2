using System;
using System.IO;
using System.Text.Json;

namespace RenameIt
{
    public class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RenameIt",
            "settings.json");

        // General Settings
        public bool RecursiveDefault { get; set; } = false;
        public bool BackupDefault { get; set; } = false;
        public string BackupFolder { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "RenameIt_Backups");

        // API Keys
        public string TmdbApiKey { get; set; } = string.Empty;
        public string TvdbApiKey { get; set; } = string.Empty;

        // Format Settings
        public string DefaultFormatPattern { get; set; } = "{n} - {s00e00} - {t}";

        // Appearance
        public string Theme { get; set; } = "System";

        // Advanced
        public bool ShowHiddenFiles { get; set; } = false;
        public bool SkipDuplicates { get; set; } = false;

        // Templates Database
        public string TemplatesDatabasePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RenameIt",
            "templates.db");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                // If loading fails, return default settings
            }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Silently fail if we can't save settings
            }
        }
    }
}
