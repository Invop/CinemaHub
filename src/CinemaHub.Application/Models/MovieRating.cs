namespace CinemaHub.Application.Models;

public class MovieRating
{
    public required Guid MovieId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
    public required Guid UserId { get; init; }
}
