using CinemaHub.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Application.Database;

public class MoviesDbContext : DbContext
{
    public MoviesDbContext(DbContextOptions<MoviesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieRating> MovieRatings { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}