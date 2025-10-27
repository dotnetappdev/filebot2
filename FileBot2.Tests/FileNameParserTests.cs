using Xunit;
using FileBot2;

namespace FileBot2.Tests
{
    public class FileNameParserTests
    {
        private readonly FileNameParser _parser;

        public FileNameParserTests()
        {
            _parser = new FileNameParser();
        }

        [Fact]
        public void ParseFileName_TVShow_StandardFormat_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "Breaking.Bad.S01E02.Cat's.in.the.Bag.mkv";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsTV);
            Assert.Equal("Breaking Bad", metadata.ShowName);
            Assert.Equal(1, metadata.Season);
            Assert.Equal(2, metadata.Episode);
            Assert.Equal("Cat's In The Bag", metadata.EpisodeTitle);
            Assert.Equal(".mkv", metadata.Extension);
        }

        [Fact]
        public void ParseFileName_TVShow_LowercaseFormat_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "the.office.s02e05.halloween.avi";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsTV);
            Assert.Equal("The Office", metadata.ShowName);
            Assert.Equal(2, metadata.Season);
            Assert.Equal(5, metadata.Episode);
            Assert.Equal("Halloween", metadata.EpisodeTitle);
        }

        [Fact]
        public void ParseFileName_TVShow_AlternateFormat_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "Friends - 1x02 - The One with the Sonogram.mp4";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsTV);
            Assert.Equal("Friends", metadata.ShowName);
            Assert.Equal(1, metadata.Season);
            Assert.Equal(2, metadata.Episode);
            Assert.Equal("The One With The Sonogram", metadata.EpisodeTitle);
        }

        [Fact]
        public void ParseFileName_Movie_WithYear_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "The.Matrix.1999.1080p.BluRay.mkv";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsMovie);
            Assert.Equal("The Matrix", metadata.MovieName);
            Assert.Equal(1999, metadata.Year);
        }

        [Fact]
        public void ParseFileName_Movie_SimpleFormat_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "Inception.2010.mkv";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsMovie);
            Assert.Equal("Inception", metadata.MovieName);
            Assert.Equal(2010, metadata.Year);
        }

        [Fact]
        public void ParseFileName_TVShow_WithoutTitle_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "Game.Of.Thrones.S08E06.mkv";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsTV);
            Assert.Equal("Game Of Thrones", metadata.ShowName);
            Assert.Equal(8, metadata.Season);
            Assert.Equal(6, metadata.Episode);
            Assert.Equal(string.Empty, metadata.EpisodeTitle);
        }

        [Fact]
        public void ParseFileName_TVShow_UnderscoreSeparators_ShouldParseCorrectly()
        {
            // Arrange
            string fileName = "Breaking_Bad_S01E02_Title.mkv";

            // Act
            var metadata = _parser.ParseFileName(fileName);

            // Assert
            Assert.True(metadata.IsTV);
            Assert.Equal("Breaking Bad", metadata.ShowName);
            Assert.Equal(1, metadata.Season);
            Assert.Equal(2, metadata.Episode);
        }
    }
}
