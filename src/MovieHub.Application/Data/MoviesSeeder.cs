using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MovieHub.Application.Data;
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
                var moviesJson = await File.ReadAllTextAsync("movies.json");
                var movies = JsonConvert.DeserializeObject<List<MovieSeed>>(moviesJson);

                // Track existing slugs to skip duplicates
                var existingSlugs = new HashSet<string>(await context.Movies.Select(m => m.Slug).ToListAsync());

                foreach (var movieSeed in movies!)
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
                    };

                    movie.Genres = movieSeed.Genres.Select(genre => new Genre
                    {
                        MovieId = movie.Id,
                        Name = genre
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
    }

    public class MovieSeed
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int YearOfRelease { get; set; }
        public List<string> Genres { get; set; } = new();
    }
}