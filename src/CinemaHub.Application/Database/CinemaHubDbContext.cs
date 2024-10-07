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
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movies");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Slug).IsRequired();

            entity.Property(e => e.Title).IsRequired();

            entity.Property(e => e.YearOfRelease).IsRequired();

            entity.Ignore(e => e.Genres);  // Ignored because genres are mapped separately
        });

        modelBuilder.Entity<MovieRating>(entity =>
        {
            entity.ToTable("ratings");

            entity.HasKey(e => new { e.UserId, e.MovieId });

            entity.Property(e => e.Rating).IsRequired();

            entity.HasOne<Movie>()
                .WithMany()
                .HasForeignKey(e => e.MovieId);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("genres");

            entity.HasKey(e => new { e.MovieId, e.Name });

            entity.Property(e => e.Name).IsRequired();

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.Genres)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}