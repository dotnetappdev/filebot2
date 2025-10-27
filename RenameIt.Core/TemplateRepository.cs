using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace RenameIt.Core
{
    public class TemplateRepository
    {
        private readonly string _databasePath;
        private readonly string _connectionString;

        public TemplateRepository(string databasePath)
        {
            _databasePath = databasePath;
            _connectionString = $"Data Source={_databasePath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var directory = Path.GetDirectoryName(_databasePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS RenameTemplates (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Pattern TEXT NOT NULL,
                    Description TEXT,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                )";
            command.ExecuteNonQuery();
        }

        public List<RenameTemplate> GetAll()
        {
            var templates = new List<RenameTemplate>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Pattern, Description, CreatedAt, UpdatedAt FROM RenameTemplates ORDER BY Name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                templates.Add(new RenameTemplate
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Pattern = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return templates;
        }

        public RenameTemplate? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Pattern, Description, CreatedAt, UpdatedAt FROM RenameTemplates WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new RenameTemplate
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Pattern = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = DateTime.Parse(reader.GetString(5))
                };
            }

            return null;
        }

        public int Add(RenameTemplate template)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO RenameTemplates (Name, Pattern, Description, CreatedAt, UpdatedAt)
                VALUES (@name, @pattern, @description, @createdAt, @updatedAt);
                SELECT last_insert_rowid();";

            var now = DateTime.UtcNow;
            command.Parameters.AddWithValue("@name", template.Name);
            command.Parameters.AddWithValue("@pattern", template.Pattern);
            command.Parameters.AddWithValue("@description", template.Description ?? string.Empty);
            command.Parameters.AddWithValue("@createdAt", now.ToString("O"));
            command.Parameters.AddWithValue("@updatedAt", now.ToString("O"));

            var id = Convert.ToInt32(command.ExecuteScalar());
            template.Id = id;
            template.CreatedAt = now;
            template.UpdatedAt = now;

            return id;
        }

        public void Update(RenameTemplate template)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE RenameTemplates
                SET Name = @name, Pattern = @pattern, Description = @description, UpdatedAt = @updatedAt
                WHERE Id = @id";

            var now = DateTime.UtcNow;
            command.Parameters.AddWithValue("@id", template.Id);
            command.Parameters.AddWithValue("@name", template.Name);
            command.Parameters.AddWithValue("@pattern", template.Pattern);
            command.Parameters.AddWithValue("@description", template.Description ?? string.Empty);
            command.Parameters.AddWithValue("@updatedAt", now.ToString("O"));

            command.ExecuteNonQuery();
            template.UpdatedAt = now;
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM RenameTemplates WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        public void SeedDefaultTemplates()
        {
            var templates = GetAll();
            if (templates.Count == 0)
            {
                // Add some default templates
                Add(new RenameTemplate
                {
                    Name = "TV Show - Standard",
                    Pattern = "{n} - {s00e00} - {t}",
                    Description = "Standard TV show format (e.g., Breaking Bad - S01E02 - Cat's in the Bag)"
                });

                Add(new RenameTemplate
                {
                    Name = "TV Show - Compact",
                    Pattern = "{n} {sxe} {t}",
                    Description = "Compact TV show format (e.g., Breaking Bad 1x02 Cat's in the Bag)"
                });

                Add(new RenameTemplate
                {
                    Name = "Movie - Standard",
                    Pattern = "{n} ({y})",
                    Description = "Standard movie format (e.g., The Matrix (1999))"
                });

                Add(new RenameTemplate
                {
                    Name = "TV Show - Plex",
                    Pattern = "{n}/Season {s00}/{n} - {s00e00} - {t}",
                    Description = "Plex-compatible folder structure for TV shows"
                });

                Add(new RenameTemplate
                {
                    Name = "TV Show - Custom Season",
                    Pattern = "{n} - Season {s} Episode {e}",
                    Description = "Custom TV show format with full words"
                });
            }
        }
    }
}
