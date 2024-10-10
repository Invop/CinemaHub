using System.Buffers.Text;
using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    
    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Genres)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");
        
        RuleFor(x => x.Overview)
            .MaximumLength(500)
            .WithMessage("Overview can't be longer than 500 characters.");
        
        RuleFor(x => x.PosterBase64)
            .Must(IsValidBase64)
            .WithMessage("Poster must be a valid Base64 string.");
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, token: token);

        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }
    private bool IsValidBase64(string? base64Text)
    {
        if (base64Text == null) return true; // Allow null values

        return Base64.IsValid(base64Text.AsSpan());
    }
}
