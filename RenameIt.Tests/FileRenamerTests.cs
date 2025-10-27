using Xunit;
using RenameIt;

namespace RenameIt.Tests
{
    public class FileRenamerTests
    {
        private readonly FileRenamer _renamer;

        public FileRenamerTests()
        {
            _renamer = new FileRenamer("TheMovieDB");
        }

        [Fact]
        public void ApplyFormat_TVShow_StandardPattern_ShouldRenameCorrectly()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsTV = true,
                ShowName = "Breaking Bad",
                Season = 1,
                Episode = 2,
                EpisodeTitle = "Cat's in the Bag",
                Extension = ".mkv"
            };
            string pattern = "{n} - {s00e00} - {t}";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.mkv");

            // Assert
            Assert.Equal("Breaking Bad - S01E02 - Cat's in the Bag.mkv", result);
        }

        [Fact]
        public void ApplyFormat_TVShow_AlternatePattern_ShouldRenameCorrectly()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsTV = true,
                ShowName = "The Office",
                Season = 2,
                Episode = 5,
                EpisodeTitle = "Halloween",
                Extension = ".avi"
            };
            string pattern = "{n} {sxe} {t}";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.avi");

            // Assert
            Assert.Equal("The Office 2x05 Halloween.avi", result);
        }

        [Fact]
        public void ApplyFormat_Movie_WithYear_ShouldRenameCorrectly()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsMovie = true,
                MovieName = "The Matrix",
                Year = 1999,
                Extension = ".mkv"
            };
            string pattern = "{n} ({y})";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.mkv");

            // Assert
            Assert.Equal("The Matrix (1999).mkv", result);
        }

        [Fact]
        public void ApplyFormat_TVShow_SeasonEpisodePadding_ShouldRenameCorrectly()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsTV = true,
                ShowName = "Friends",
                Season = 1,
                Episode = 2,
                EpisodeTitle = "The One with the Sonogram",
                Extension = ".mp4"
            };
            string pattern = "{n} - Season {s00} Episode {e00} - {t}";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.mp4");

            // Assert
            Assert.Equal("Friends - Season 01 Episode 02 - The One with the Sonogram.mp4", result);
        }

        [Fact]
        public void ApplyFormat_EmptyPattern_ShouldReturnOriginal()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsTV = true,
                ShowName = "Test Show",
                Extension = ".mkv"
            };
            string pattern = "";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.mkv");

            // Assert
            Assert.Equal("original.mkv", result);
        }

        [Fact]
        public void ApplyFormat_WithExtensionToken_ShouldIncludeExtension()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsMovie = true,
                MovieName = "Inception",
                Year = 2010,
                Extension = ".mp4"
            };
            string pattern = "{n}.{y}.{ext}";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.mp4");

            // Assert - extension is added at the end automatically
            Assert.Equal("Inception.2010.mp4", result);
        }

        [Fact]
        public void ApplyFormat_InvalidCharacters_ShouldRemoveThem()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsTV = true,
                ShowName = "Test: Show?",
                Season = 1,
                Episode = 1,
                EpisodeTitle = "Title/Test",
                Extension = ".mkv"
            };
            string pattern = "{n} - {s00e00} - {t}";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "original.mkv");

            // Assert
            Assert.DoesNotContain(":", result);
            Assert.DoesNotContain("?", result);
            Assert.DoesNotContain("/", result);
        }

        [Fact]
        public void ApplyFormat_OriginalFilenameToken_ShouldIncludeOriginalName()
        {
            // Arrange
            var metadata = new FileMetadata
            {
                IsTV = true,
                ShowName = "Breaking Bad",
                Season = 1,
                Episode = 2,
                Extension = ".mkv"
            };
            string pattern = "{n} - {s00e00} - [{fn}]";

            // Act
            string result = _renamer.ApplyFormat(pattern, metadata, "breaking.bad.s01e02.720p.mkv");

            // Assert
            Assert.Equal("Breaking Bad - S01E02 - [breaking.bad.s01e02.720p].mkv", result);
        }
    }
}
