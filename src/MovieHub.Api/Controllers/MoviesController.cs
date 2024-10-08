using Asp.Versioning;
using MovieHub.Api.Mapping;
using MovieHub.Application.Infrastructure.Services;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;

namespace MovieHub.Api.Controllers;
[ApiVersion(1.0)]
[Authorize]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, IIdentityService identityService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _identityService = identityService;
        _logger = logger;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token);
        // await _outputCacheStore.EvictByTagAsync("movies", token);
        var movieResponse = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movieResponse);
    }
    

    [HttpGet(ApiEndpoints.Movies.Get)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userIdStr = _identityService.GetUserIdentity();
        Guid? userId = null;

        if (string.IsNullOrEmpty(userIdStr))
        {
            _logger.LogWarning("User identity is null or empty. Proceeding without user ID.");
        }
        else
        {
            if (!Guid.TryParse(userIdStr, out var parsedUserId))
            {
                _logger.LogError("User identity is not a valid GUID. Actual value: {UserIdStr}", userIdStr);
                throw new InvalidOperationException("User identity is not a valid GUID.");
            }
            userId = parsedUserId;
        }
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {
        var userIdStr = _identityService.GetUserIdentity();
        Guid? userId = null;

        if (string.IsNullOrEmpty(userIdStr))
        {
            _logger.LogWarning("User identity is null or empty. Proceeding without user ID.");
        }
        else
        {
            if (!Guid.TryParse(userIdStr, out var parsedUserId))
            {
                _logger.LogError("User identity is not a valid GUID. Actual value: {UserIdStr}", userIdStr);
                throw new InvalidOperationException("User identity is not a valid GUID.");
            }
            userId = parsedUserId;
        }
        _logger.LogInformation(userId.ToString());
        var options = request.MapToOptions().WithUser(userId);
        var movies = await _movieService.GetAllAsync(options, token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
        var moviesResponse = movies.MapToResponse(request.Page, request.PageSize, movieCount);
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var userId = Guid.Parse(_identityService.GetUserIdentity());
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        // await _outputCacheStore.EvictByTagAsync("movies", token);
        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, token);
        if (!deleted)
        {
            return NotFound();
        }

        // await _outputCacheStore.EvictByTagAsync("movies", token);
        return Ok();
    }
}