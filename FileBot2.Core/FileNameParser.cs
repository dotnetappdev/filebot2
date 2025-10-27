using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FileBot2
{
    public class FileNameParser
    {
        // Compiled regex patterns for better performance
        private static readonly Regex TvShowRegex = new Regex(
            @"^(.+?)[\s\._\-]+[Ss]?(\d{1,2})[Ee](\d{2,3})(?:[\s\._\-]+(.+))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex AlternateTvRegex = new Regex(
            @"^(.+?)[\s\-]+(\d{1,2})[xX](\d{2})(?:[\s\-]+(.+?))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MovieRegex = new Regex(
            @"^(.+?)[\s\._\-]+(\d{4})(?:[\s\._\-]+.+)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CleanNameRegex = new Regex(
            @"\s+",
            RegexOptions.Compiled);

        public FileMetadata ParseFileName(string fileName)
        {
            var metadata = new FileMetadata
            {
                OriginalFileName = fileName
            };

            // Remove extension
            string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
            metadata.Extension = System.IO.Path.GetExtension(fileName);

            // Try to parse TV show format: Show.Name.S01E02.Title
            var tvShowMatch = TvShowRegex.Match(nameWithoutExt);

            if (tvShowMatch.Success)
            {
                metadata.ShowName = CleanName(tvShowMatch.Groups[1].Value);
                metadata.Season = int.Parse(tvShowMatch.Groups[2].Value);
                metadata.Episode = int.Parse(tvShowMatch.Groups[3].Value);
                metadata.EpisodeTitle = tvShowMatch.Groups.Count > 4 && !string.IsNullOrEmpty(tvShowMatch.Groups[4].Value) 
                    ? CleanName(tvShowMatch.Groups[4].Value) 
                    : string.Empty;
                metadata.IsTV = true;
                return metadata;
            }

            // Try alternative format: Show Name - 1x02 - Title
            var altTvMatch = AlternateTvRegex.Match(nameWithoutExt);

            if (altTvMatch.Success)
            {
                metadata.ShowName = CleanName(altTvMatch.Groups[1].Value);
                metadata.Season = int.Parse(altTvMatch.Groups[2].Value);
                metadata.Episode = int.Parse(altTvMatch.Groups[3].Value);
                metadata.EpisodeTitle = altTvMatch.Groups.Count > 4 && !string.IsNullOrEmpty(altTvMatch.Groups[4].Value)
                    ? CleanName(altTvMatch.Groups[4].Value)
                    : string.Empty;
                metadata.IsTV = true;
                return metadata;
            }

            // Try to parse movie format: Movie.Name.2020.1080p.BluRay
            var movieMatch = MovieRegex.Match(nameWithoutExt);

            if (movieMatch.Success)
            {
                metadata.MovieName = CleanName(movieMatch.Groups[1].Value);
                metadata.Year = int.Parse(movieMatch.Groups[2].Value);
                metadata.IsMovie = true;
                return metadata;
            }

            // Default: use the filename as-is
            metadata.MovieName = CleanName(nameWithoutExt);
            metadata.IsMovie = true;
            return metadata;
        }

        private string CleanName(string name)
        {
            // Replace dots, underscores with spaces
            name = name.Replace('.', ' ').Replace('_', ' ');
            
            // Remove extra spaces using compiled regex
            name = CleanNameRegex.Replace(name, " ");
            
            // Trim
            name = name.Trim();

            // Title case
            if (!string.IsNullOrEmpty(name))
            {
                var words = name.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 0)
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                    }
                }
                name = string.Join(" ", words);
            }

            return name;
        }
    }

    public class FileMetadata
    {
        public string OriginalFileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        
        // TV Show properties
        public bool IsTV { get; set; }
        public string ShowName { get; set; } = string.Empty;
        public int Season { get; set; }
        public int Episode { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        
        // Movie properties
        public bool IsMovie { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public int Year { get; set; }
        
        // Additional metadata
        public string Quality { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}
