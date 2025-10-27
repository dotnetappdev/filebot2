using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileBot2
{
    public interface IMetadataProvider
    {
        Task<ShowMetadata?> SearchShowAsync(string showName);
        Task<EpisodeMetadata?> GetEpisodeAsync(string showName, int season, int episode);
        Task<MovieMetadata?> SearchMovieAsync(string movieName, int? year = null);
    }

    public class TheMovieDBProvider : IMetadataProvider
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "demo"; // In a real app, this would be from config

        public TheMovieDBProvider()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        }

        public async Task<ShowMetadata?> SearchShowAsync(string showName)
        {
            try
            {
                // Simulate API call (in real implementation, would call actual API)
                await Task.Delay(100);
                
                return new ShowMetadata
                {
                    Name = showName,
                    Year = 2020,
                    Id = "12345"
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<EpisodeMetadata?> GetEpisodeAsync(string showName, int season, int episode)
        {
            try
            {
                // Simulate API call
                await Task.Delay(100);
                
                return new EpisodeMetadata
                {
                    ShowName = showName,
                    Season = season,
                    Episode = episode,
                    Title = $"Episode {episode}",
                    AirDate = DateTime.Now
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<MovieMetadata?> SearchMovieAsync(string movieName, int? year = null)
        {
            try
            {
                // Simulate API call
                await Task.Delay(100);
                
                return new MovieMetadata
                {
                    Title = movieName,
                    Year = year ?? 2020,
                    Id = "67890"
                };
            }
            catch
            {
                return null;
            }
        }
    }

    public class TVMazeProvider : IMetadataProvider
    {
        private readonly HttpClient _httpClient;

        public TVMazeProvider()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.tvmaze.com/");
        }

        public async Task<ShowMetadata?> SearchShowAsync(string showName)
        {
            try
            {
                await Task.Delay(100);
                return new ShowMetadata { Name = showName, Year = 2020, Id = "tv123" };
            }
            catch
            {
                return null;
            }
        }

        public async Task<EpisodeMetadata?> GetEpisodeAsync(string showName, int season, int episode)
        {
            try
            {
                await Task.Delay(100);
                return new EpisodeMetadata
                {
                    ShowName = showName,
                    Season = season,
                    Episode = episode,
                    Title = $"Episode {episode}"
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<MovieMetadata?> SearchMovieAsync(string movieName, int? year = null)
        {
            await Task.Delay(100);
            return new MovieMetadata { Title = movieName, Year = year ?? 2020, Id = "movie123" };
        }
    }

    public class TheTVDBProvider : IMetadataProvider
    {
        private readonly HttpClient _httpClient;

        public TheTVDBProvider()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.thetvdb.com/");
        }

        public async Task<ShowMetadata?> SearchShowAsync(string showName)
        {
            try
            {
                await Task.Delay(100);
                return new ShowMetadata { Name = showName, Year = 2020, Id = "tvdb123" };
            }
            catch
            {
                return null;
            }
        }

        public async Task<EpisodeMetadata?> GetEpisodeAsync(string showName, int season, int episode)
        {
            try
            {
                await Task.Delay(100);
                return new EpisodeMetadata
                {
                    ShowName = showName,
                    Season = season,
                    Episode = episode,
                    Title = $"Episode {episode}"
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<MovieMetadata?> SearchMovieAsync(string movieName, int? year = null)
        {
            await Task.Delay(100);
            return new MovieMetadata { Title = movieName, Year = year ?? 2020, Id = "tvdbmovie123" };
        }
    }

    public class ShowMetadata
    {
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Id { get; set; } = string.Empty;
    }

    public class EpisodeMetadata
    {
        public string ShowName { get; set; } = string.Empty;
        public int Season { get; set; }
        public int Episode { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime AirDate { get; set; }
    }

    public class MovieMetadata
    {
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Id { get; set; } = string.Empty;
    }
}
