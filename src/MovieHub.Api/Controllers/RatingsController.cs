﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Api.Mapping;
using MovieHub.Application.Infrastructure.Services;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;

namespace MovieHub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<RatingsController> _logger;

    public RatingsController(IRatingService ratingService,IIdentityService identityService, ILogger<RatingsController> logger)
    {
        _ratingService = ratingService;
        _identityService = identityService;
        _logger = logger;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id,
        [FromBody] RateMovieRequest request, CancellationToken token)
    {
        var userId = GetUserId();
        var result = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
        CancellationToken token)
    {
        var userId = GetUserId();
        var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token = default)
    {
        var userId = GetUserId();
        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);
        var ratingsResponse = ratings.MapToResponse();
        return Ok(ratingsResponse);
    }
    
    private Guid? GetUserId()
    {
        var userIdStr = _identityService.GetUserIdentity();
        if (string.IsNullOrEmpty(userIdStr))
        {
            _logger.LogWarning("User identity is null or empty. Proceeding without user ID.");
            return null;
        }

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            _logger.LogError("User identity is not a valid GUID. Actual value: {UserIdStr}", userIdStr);
            throw new InvalidOperationException("User identity is not a valid GUID.");
        }

        return userId;
    }
}
