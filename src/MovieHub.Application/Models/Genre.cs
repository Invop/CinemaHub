namespace MovieHub.Application.Models;

public class Genre
{
    public Guid MovieId { get; set; }
    public string Name { get; set; } = default!;

    // Navigation property
    public Movie Movie { get; set; } = default!;
}
