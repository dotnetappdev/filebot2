using System;
using System.Text.RegularExpressions;

namespace RenameIt
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

            // FileBot-compatible format patterns (based on https://www.filebot.net/naming.html)
            
            // {n} - Name (show name or movie name)
            result = result.Replace("{n}", GetName(metadata));

            // Season/Episode tokens
            // {s} - Season number
            result = result.Replace("{s}", metadata.Season.ToString());
            
            // {e} - Episode number
            result = result.Replace("{e}", metadata.Episode.ToString());
            
            // {s00} - Season number (2 digits with leading zero)
            result = result.Replace("{s00}", metadata.Season.ToString("00"));
            
            // {e00} - Episode number (2 digits with leading zero)
            result = result.Replace("{e00}", metadata.Episode.ToString("00"));
            
            // {s00e00} - Season and episode (e.g., S01E02)
            result = result.Replace("{s00e00}", $"S{metadata.Season:00}E{metadata.Episode:00}");
            
            // {sxe} - Season and episode (e.g., 1x02)
            result = result.Replace("{sxe}", $"{metadata.Season}x{metadata.Episode:00}");

            // Title tokens
            // {t} - Episode title / Movie title
            result = result.Replace("{t}", metadata.EpisodeTitle);

            // Year tokens
            // {y} - Year (4 digits)
            result = result.Replace("{y}", metadata.Year > 0 ? metadata.Year.ToString() : string.Empty);

            // Extension token
            // {ext} - File extension without dot
            result = result.Replace("{ext}", metadata.Extension.TrimStart('.'));

            // Metadata source token
            // {source} - Metadata source (e.g., TheMovieDB)
            result = result.Replace("{source}", _source);

            // Additional useful tokens
            // {fn} - Original filename without extension
            result = result.Replace("{fn}", System.IO.Path.GetFileNameWithoutExtension(originalFileName));

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
            // Use HashSet for faster character lookups
            var invalidChars = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());
            var result = new System.Text.StringBuilder(fileName.Length);

            foreach (char c in fileName)
            {
                if (invalidChars.Contains(c))
                {
                    // Skip invalid characters
                    continue;
                }
                else if (c == ':')
                {
                    result.Append(" -");
                }
                else if (c == '?')
                {
                    // Skip question marks
                }
                else if (c == '"')
                {
                    result.Append('\'');
                }
                else
                {
                    result.Append(c);
                }
            }

            // Clean up multiple spaces
            string cleaned = result.ToString();
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

            return cleaned;
        }
    }
}
