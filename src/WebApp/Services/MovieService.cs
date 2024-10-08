using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using MovieHub.Contracts.Responses;

namespace WebApp.Services
{

    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl = "/api/movies";

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<MoviesResponse> GetMovies(int page)
        {
            return _httpClient.GetFromJsonAsync<MoviesResponse>(_remoteServiceBaseUrl+$"?page={page}")!;
        }
    }
}