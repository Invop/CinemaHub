using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public interface IGenreService
{
    Task<IEnumerable<GenreLookup>> GetAllGenresAsync();
    Task<GenreLookup?> GetGenreByIdAsync(int id);
    Task CreateGenreAsync(GenreLookup genre);
    Task UpdateGenreAsync(GenreLookup genre);
    Task DeleteGenreAsync(int id);
}