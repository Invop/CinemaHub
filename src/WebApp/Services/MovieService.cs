using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApp.Services
{
    public class MoviesResponse
    {
        public List<Movie> Items { get; set; } = [];
        public int PageSize { get; init; }
        public int Page { get; init; }
        public int Total { get; init; }
        public bool HasNextPage { get; init; }
    }

    public class Movie
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public float? Rating { get; init; }
        public int? UserRating { get; init; }
        public int YearOfRelease { get; init; }
        public IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
    }

    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl = "/api/movies";

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<MoviesResponse> GetMovies()
        {
            return _httpClient.GetFromJsonAsync<MoviesResponse>(_remoteServiceBaseUrl)!;
        }
    }
}