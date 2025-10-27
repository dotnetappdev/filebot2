using System;
using System.Text.RegularExpressions;

namespace FileBot2
{
    public class FileRenamer
    {
        private readonly string _source;

        public FileRenamer(string source)
        {
            _source = source;
        }

        public string ApplyFormat(string pattern, FileMetadata metadata, string originalFileName)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return originalFileName;
            }

            string result = pattern;

            // FileBot-compatible format patterns
            // {n} - Name (show name or movie name)
            result = result.Replace("{n}", GetName(metadata));

            // {s} - Season number
            result = result.Replace("{s}", metadata.Season.ToString());

            // {e} - Episode number
            result = result.Replace("{e}", metadata.Episode.ToString());

            // {s00} - Season number (2 digits)
            result = result.Replace("{s00}", metadata.Season.ToString("00"));

            // {e00} - Episode number (2 digits)
            result = result.Replace("{e00}", metadata.Episode.ToString("00"));

            // {s00e00} - Season and episode (e.g., S01E02)
            result = result.Replace("{s00e00}", $"S{metadata.Season:00}E{metadata.Episode:00}");

            // {sxe} - Season and episode (e.g., 1x02)
            result = result.Replace("{sxe}", $"{metadata.Season}x{metadata.Episode:00}");

            // {t} - Episode title
            result = result.Replace("{t}", metadata.EpisodeTitle);

            // {y} - Year
            result = result.Replace("{y}", metadata.Year > 0 ? metadata.Year.ToString() : string.Empty);

            // {source} - Source (e.g., TheMovieDB)
            result = result.Replace("{source}", _source);

            // {ext} - Extension
            result = result.Replace("{ext}", metadata.Extension.TrimStart('.'));

            // Clean up any remaining empty spaces or multiple spaces
            result = Regex.Replace(result, @"\s+", " ").Trim();

            // Clean up invalid characters for filenames
            result = CleanFileName(result);

            // Add extension if not already present
            if (!result.EndsWith(metadata.Extension))
            {
                result += metadata.Extension;
            }

            return result;
        }

        private string GetName(FileMetadata metadata)
        {
            if (metadata.IsTV && !string.IsNullOrEmpty(metadata.ShowName))
            {
                return metadata.ShowName;
            }
            else if (metadata.IsMovie && !string.IsNullOrEmpty(metadata.MovieName))
            {
                return metadata.MovieName;
            }
            return "Unknown";
        }

        private string CleanFileName(string fileName)
        {
            // Remove invalid characters
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }

            // Replace some characters with spaces for readability
            fileName = fileName.Replace(":", " -");
            fileName = fileName.Replace("?", "");
            fileName = fileName.Replace("\"", "'");

            // Clean up multiple spaces
            fileName = Regex.Replace(fileName, @"\s+", " ").Trim();

            return fileName;
        }
    }
}
