using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaHub.Application.Models;

public class MovieRating
{
    [Key, Column(Order = 0)]
    public required Guid MovieId { get; init; }

    [Required]
    public required string Slug { get; init; }

    [Required]
    public required int Rating { get; init; }

    [Key, Column(Order = 1)]
    public required Guid UserId { get; init; }
}