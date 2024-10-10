using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieHub.Application.Models;
using Newtonsoft.Json;

namespace MovieHub.Application.Data
{
    public class MoviesSeeder : IDbSeeder<MovieDbContext>
    {
        private readonly ILogger<MoviesSeeder> _logger;

        public MoviesSeeder(ILogger<MoviesSeeder> logger)
        {
            _logger = logger;
        }

        public async Task SeedAsync(MovieDbContext context)
        {
            if (!await context.Movies.AnyAsync())
            {
                var moviesJson = await File.ReadAllTextAsync("reduced_movies.json");
                var movies = JsonConvert.DeserializeObject<List<MovieSeed>>(moviesJson);

                var allGenres = movies!
                    .SelectMany(m => m.Genres)
                    .Select(NormalizeGenre)
                    .Distinct()
                    .ToList();

                var genreLookup = new Dictionary<string, GenreLookup>();

                foreach (var genre in allGenres)
                {
                    var genreEntity = await FindOrCreateGenreAsync(context, genre);
                    genreLookup[genre] = genreEntity;
                }

                await context.SaveChangesAsync();

                // Track existing slugs to skip duplicates
                var existingSlugs = new HashSet<string>(await context.Movies.Select(m => m.Slug).ToListAsync());

                foreach (var movieSeed in movies)
                {
                    if (existingSlugs.Contains(movieSeed.Slug))
                    {
                        _logger.LogInformation($"Duplicate slug '{movieSeed.Slug}' found. Skipping.");
                        continue;
                    }

                    var movie = new Movie
                    {
                        Id = movieSeed.Id,
                        Title = movieSeed.Title,
                        YearOfRelease = movieSeed.YearOfRelease,
                        Slug = movieSeed.Slug,
                        PosterBase64 = movieSeed.PosterBase64,
                    };

                    movie.Genres = movieSeed.Genres.Select(genreName => new Genre
                    {
                        MovieId = movie.Id,
                        GenreLookup = genreLookup[NormalizeGenre(genreName)]
                    }).ToList();

                    existingSlugs.Add(movieSeed.Slug);
                    await context.Movies.AddAsync(movie);
                }

                await context.SaveChangesAsync();

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Movies have been seeded.");
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Movies already exist in the database.");
                }
            }
        }

        private async Task<GenreLookup> FindOrCreateGenreAsync(MovieDbContext context, string genre)
        {
            var normalizedGenre = NormalizeGenre(genre);
            var genreEntity = await context.Genres.FirstOrDefaultAsync(g => g.Name == normalizedGenre);

            if (genreEntity == null)
            {
                genreEntity = new GenreLookup { Name = normalizedGenre };
                await context.Genres.AddAsync(genreEntity);
            }

            return genreEntity;
        }

        private string NormalizeGenre(string genre)
        {
            return genre.Trim().ToLower();
        }
    }

    public class MovieSeed
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int YearOfRelease { get; set; }
        public List<string> Genres { get; set; } = new();
        public string PosterBase64 {get; set;} = default!;
    }
}