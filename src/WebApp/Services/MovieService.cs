using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;

namespace WebApp.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private const string MovieEndpoint = "/api/movies";
        private const string MovieRatingsEndpoint = "/api/ratings";

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<MovieResponse?> GetMovie(string idOrSlug)
        {
            return _httpClient.GetFromJsonAsync<MovieResponse>($"{MovieEndpoint}/{idOrSlug}");
        }

        public Task<MoviesResponse> GetMovies(GetAllMoviesRequest request)
        {
            var queryString = BuildQueryString(request);
            return _httpClient.GetFromJsonAsync<MoviesResponse>(MovieEndpoint + queryString)!;
        }

        public Task CreateMovie(CreateMovieRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, MovieEndpoint);
            requestMessage.Content = JsonContent.Create(request);
            return _httpClient.SendAsync(requestMessage);
        }

        public Task UpdateMovie(Guid id, UpdateMovieRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{MovieEndpoint}/{id}");
            requestMessage.Content = JsonContent.Create(request);
            return _httpClient.SendAsync(requestMessage);
        }

        public Task DeleteMovie(Guid id)
        {
            return _httpClient.DeleteAsync($"{MovieEndpoint}/{id}");
        }

        
        public Task RateMovie(Guid id, RateMovieRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{MovieRatingsEndpoint}/{id}");
            requestMessage.Content = JsonContent.Create(request);
            return _httpClient.SendAsync(requestMessage);
        }

        public Task DeleteRating(Guid id)
        {
            return _httpClient.DeleteAsync($"{MovieRatingsEndpoint}/{id}");
        }
        
        public Task<MovieRatingResponse[]?> GetUserRatings()
        {
            return _httpClient.GetFromJsonAsync<MovieRatingResponse[]>($"{MovieRatingsEndpoint}/me");
        }
        
        
        private string BuildQueryString(GetAllMoviesRequest request)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.Title))
            {
                queryParams.Add($"title={request.Title}");
            }

            if (request.Year.HasValue)
            {
                queryParams.Add($"year={request.Year}");
            }

            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                queryParams.Add($"sortBy={request.SortBy}");
            }

            return "?" + string.Join("&", queryParams);
        }
    }
}