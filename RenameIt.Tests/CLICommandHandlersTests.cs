using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using RenameIt.CLI;

namespace RenameIt.Tests
{
    public class CLICommandHandlersTests : IDisposable
    {
        private readonly string _testDirectory;

        public CLICommandHandlersTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "RenameItCLITests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public async Task RenameAsync_WithDryRun_DoesNotRenameFiles()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "breaking.bad.s01e01.mkv");
            File.WriteAllText(testFile, "test content");
            var pattern = "{n} - {s00e00}";

            // Act
            var result = await CommandHandlers.RenameAsync(_testDirectory, pattern, "TheMovieDB", false, false, true);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(testFile)); // Original file should still exist
            Assert.Single(Directory.GetFiles(_testDirectory));
        }

        [Fact]
        public async Task RenameAsync_WithValidInput_RenamesFiles()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "breaking.bad.s01e01.mkv");
            File.WriteAllText(testFile, "test content");
            var pattern = "{n} - {s00e00}";

            // Act
            var result = await CommandHandlers.RenameAsync(_testDirectory, pattern, "TheMovieDB", false, false, false);

            // Assert
            Assert.Equal(0, result);
            Assert.False(File.Exists(testFile)); // Original file should be renamed
            var renamedFile = Path.Combine(_testDirectory, "Breaking Bad - S01E01.mkv");
            Assert.True(File.Exists(renamedFile));
        }

        [Fact]
        public async Task RenameAsync_WithBackup_CreatesBackupFile()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "breaking.bad.s01e01.mkv");
            File.WriteAllText(testFile, "test content");
            var pattern = "{n} - {s00e00}";

            // Act
            var result = await CommandHandlers.RenameAsync(_testDirectory, pattern, "TheMovieDB", false, true, false);

            // Assert
            Assert.Equal(0, result);
            var backupFile = testFile + ".backup";
            Assert.True(File.Exists(backupFile));
        }

        [Fact]
        public async Task RenameAsync_WithNonExistentPath_ReturnsError()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDirectory, "does-not-exist");
            var pattern = "{n} - {s00e00}";

            // Act
            var result = await CommandHandlers.RenameAsync(nonExistentPath, pattern, "TheMovieDB", false, false, false);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task PreviewAsync_ReturnsSuccess()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "breaking.bad.s01e01.mkv");
            File.WriteAllText(testFile, "test content");
            var pattern = "{n} - {s00e00}";

            // Act
            var result = await CommandHandlers.PreviewAsync(_testDirectory, pattern, "TheMovieDB", false);

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(testFile)); // Original file should still exist
        }

        [Fact]
        public async Task BatchAsync_WithNonExistentScript_ReturnsError()
        {
            // Arrange
            var nonExistentScript = Path.Combine(_testDirectory, "nonexistent-script.txt");

            // Act
            var result = await CommandHandlers.BatchAsync(nonExistentScript, false);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task BatchAsync_WithValidScript_ProcessesCommands()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "breaking.bad.s01e01.mkv");
            File.WriteAllText(testFile, "test content");

            var scriptPath = Path.Combine(_testDirectory, "test-script.txt");
            var scriptContent = $@"
[Test Rename]
input={_testDirectory}
pattern={{n}} - {{s00e00}}
source=TheMovieDB
recursive=false
backup=false
";
            File.WriteAllText(scriptPath, scriptContent);

            // Act
            var result = await CommandHandlers.BatchAsync(scriptPath, true); // Dry run

            // Assert
            Assert.Equal(0, result);
            Assert.True(File.Exists(testFile)); // Original file should still exist (dry run)
        }

        [Fact]
        public async Task RenameAsync_WithRecursive_ProcessesSubdirectories()
        {
            // Arrange
            var subDir = Path.Combine(_testDirectory, "season1");
            Directory.CreateDirectory(subDir);
            var testFile = Path.Combine(subDir, "breaking.bad.s01e01.mkv");
            File.WriteAllText(testFile, "test content");
            var pattern = "{n} - {s00e00}";

            // Act
            var result = await CommandHandlers.RenameAsync(_testDirectory, pattern, "TheMovieDB", true, false, false);

            // Assert
            Assert.Equal(0, result);
            var renamedFile = Path.Combine(subDir, "Breaking Bad - S01E01.mkv");
            Assert.True(File.Exists(renamedFile));
        }
    }
}
