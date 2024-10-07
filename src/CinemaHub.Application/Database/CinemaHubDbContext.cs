using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CinemaHub.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace CinemaHub.Application.Database
{
    public class CinemaHubDbContext : DbContext
    {
        public CinemaHubDbContext(DbContextOptions<CinemaHubDbContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieRating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Slug).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Genres).HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));
            });

            modelBuilder.Entity<MovieRating>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.MovieId }); // Composite key
            });
        }
    }
}