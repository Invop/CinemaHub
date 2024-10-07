using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CinemaHub.Application.Models;

public partial class Movie
{
    private string _slug = string.Empty;

    [Key]
    public required Guid Id { get; init; }
        
    [Required]
    public required string Title { get; set; }

    public float? Rating { get; set; }

    public int? UserRating { get; set; }

    [Required]
    public required int YearOfRelease { get; set; }

    [Required]
    public string Slug
    {
        get => GenerateSlug();
        private set => _slug = value;
    }

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}