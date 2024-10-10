using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Data;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly MovieDbContext _context;

        public GenreRepository(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GenreLookup>> GetAllAsync(CancellationToken token = default)
        {
            return await _context.GenreLookups.ToListAsync(cancellationToken: token);
        }

        public async Task<GenreLookup?> GetByIdAsync(int id, CancellationToken token = default)
        {
            return await _context.GenreLookups.FindAsync([id], token);
        }

        public async Task<GenreLookup?> GetByNameAsync(string name, CancellationToken token)
        {
            return await _context.Set<GenreLookup>()
                .FirstOrDefaultAsync(g => g.Name == name, token);
        }

        public async Task CreateAsync(GenreLookup genre, CancellationToken token = default)
        {
            _context.GenreLookups.Add(genre);
            await _context.SaveChangesAsync(token);
        }

        public async Task UpdateAsync(GenreLookup genre, CancellationToken token = default)
        {
            _context.GenreLookups.Update(genre);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(int id, CancellationToken token = default)
        {
            var genre = await _context.GenreLookups.FindAsync([id], token);
            if (genre != null)
            {
                _context.GenreLookups.Remove(genre);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}