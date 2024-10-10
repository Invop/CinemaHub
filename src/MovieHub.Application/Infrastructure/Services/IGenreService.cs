using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public interface IGenreService
{
    Task<IEnumerable<GenreLookup>> GetAllGenresAsync(CancellationToken token = default);
    Task<GenreLookup?> GetGenreByIdAsync(int id, CancellationToken token = default);
    Task<GenreLookup?> GetByNameAsync(string name, CancellationToken token);
    Task CreateGenreAsync(GenreLookup genre, CancellationToken token = default);
    Task UpdateGenreAsync(GenreLookup genre, CancellationToken token = default);
    Task DeleteGenreAsync(int id, CancellationToken token = default);
}