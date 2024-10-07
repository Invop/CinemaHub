using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CinemaHub.Application.Models;

public partial class Movie
{
    private string? _slug;
    
    public Guid Id { get; set; }
    public string Slug 
    { 
        get => _slug ??= GenerateSlug();
        private set => _slug = value;
    }
    private string _title = default!;
    public string Title 
    { 
        get => _title;
        set
        {
            _title = value;
            _slug = null; // Reset slug when title changes
        }
    }
    private int _yearOfRelease;
    public int YearOfRelease 
    { 
        get => _yearOfRelease;
        set
        {
            _yearOfRelease = value;
            _slug = null; // Reset slug when year changes
        }
    }

    [NotMapped]
    public double? Rating { get; set; }
    [NotMapped]
    public int? UserRating { get; set; }

    // Navigation properties
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<MovieRating> Ratings { get; set; } = new List<MovieRating>();

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}