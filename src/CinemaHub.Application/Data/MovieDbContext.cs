using CinemaHub.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Application.Data;

public class MovieDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieRating> Ratings => Set<MovieRating>();  // Changed to match table name

    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movies");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Slug).HasColumnName("slug").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.YearOfRelease).HasColumnName("yearofrelease").IsRequired();
            
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("movies_slug_idx");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("genres");
            entity.HasKey(e => new { e.MovieId, e.Name });
            
            entity.Property(e => e.MovieId).HasColumnName("movieid");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();

            entity.HasOne(e => e.Movie)
                .WithMany(e => e.Genres)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MovieRating>(entity =>
        {
            entity.ToTable("ratings");
            entity.HasKey(e => new { e.UserId, e.MovieId });
            
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.MovieId).HasColumnName("movieid");
            entity.Property(e => e.Rating).HasColumnName("rating").IsRequired();

            entity.HasOne(e => e.Movie)
                .WithMany(e => e.Ratings)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}