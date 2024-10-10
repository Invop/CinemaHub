using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public interface IGenreRepository
{
    Task<IEnumerable<GenreLookup>> GetAllAsync(CancellationToken token = default);
    Task<GenreLookup?> GetByIdAsync(int id, CancellationToken token = default);
    Task<GenreLookup?> GetByNameAsync(string name, CancellationToken token);
    Task CreateAsync(GenreLookup genre, CancellationToken token = default);
    Task UpdateAsync(GenreLookup genre, CancellationToken token = default);
    Task DeleteAsync(int id, CancellationToken token = default);

}