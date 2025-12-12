using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platemates.Application.Dtos;
using Platemates.Application.Interfaces;

namespace Platemates.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly IUserService _userService;
    private readonly IGooglePlacesService _googlePlacesService;

    public RestaurantsController(IRestaurantService restaurantService, IUserService userService, IGooglePlacesService googlePlacesService)
    {
        _restaurantService = restaurantService;
        _userService = userService;
        _googlePlacesService = googlePlacesService;
    }

    ///<summary>
    /// Search for restaurants using Google Places Autocomplete
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<GooglePlacesSearchResultDto>> SearchRestaurants(
        [FromQuery] string query
    )
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new { error = "Query parameter is required" });
        }

        try
        {
            var user = await GetOrCreateCurrentUserAsync();
            var results = await _googlePlacesService.SearchRestaurantsAsync(query);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to search restaurants", details = ex.Message });
        }
    }

    /// <summary>
    /// Add a restaurant manually to "Want to Go" list
    /// </summary>
    [HttpPost("manual")]
    public async Task<ActionResult<UserRestaurantDto>> AddRestaurantManually(
        [FromBody] AddRestaurantManuallyDto dto)
    {
        var user = await GetOrCreateCurrentUserAsync();
        var userRestaurant = await _restaurantService.AddRestaurantManuallyAsync(user.Id, dto);

        return CreatedAtAction(
            nameof(AddRestaurantManually),
            new { id = userRestaurant.Id },
            userRestaurant
        );
    }

    /// <summary>
    /// Get current user's restaurants
    /// </summary>
    /// <param name="status">Filter by status: "all", "want-to-go", or "been-to" (default: "all")</param>
    [HttpGet("my-list")]
    public async Task<ActionResult<List<UserRestaurantDto>>> GetMyRestaurants(
        [FromQuery] string status = "all")
    {
        var user = await GetOrCreateCurrentUserAsync();
        var restaurants = await _restaurantService.GetMyRestaurantsAsync(user.Id, status);

        return Ok(restaurants);
    }

    /// <summary>
    /// Mark a restaurant as "Been To" and add a review
    /// </summary>
    [HttpPost("{userRestaurantId}/mark-as-been-to")]
    public async Task<ActionResult<UserRestaurantDto>> MarkAsBeenTo(
        Guid userRestaurantId,
        [FromBody] AddReviewDto reviewDto)
    {
        try
        {
            var user = await GetOrCreateCurrentUserAsync();
            var userRestaurant = await _restaurantService.MarkAsBeenToAsync(user.Id, userRestaurantId, reviewDto);

            return Ok(userRestaurant);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Mark a restaurant back to "Want to Go" (deletes review)
    /// </summary>
    [HttpDelete("{userRestaurantId}/review")]
    public async Task<ActionResult<UserRestaurantDto>> MarkAsWantToGo(Guid userRestaurantId)
    {
        try
        {
            var user = await GetOrCreateCurrentUserAsync();
            var userRestaurant = await _restaurantService.MarkAsWantToGoAsync(user.Id, userRestaurantId);

            return Ok(userRestaurant);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update restaurant information (PATCH - only updates provided fields)
    /// </summary>
    [HttpPatch("{userRestaurantId}")]
    public async Task<ActionResult<UserRestaurantDto>> UpdateRestaurant(
        Guid userRestaurantId,
        [FromBody] UpdateRestaurantDto dto)
    {
        try
        {
            var user = await GetOrCreateCurrentUserAsync();
            var userRestaurant = await _restaurantService.UpdateRestaurantAsync(user.Id, userRestaurantId, dto);

            return Ok(userRestaurant);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Helper method: Get or create current user from JWT token
    /// </summary>
    private async Task<UserResponseDto> GetOrCreateCurrentUserAsync()
    {
        // Get Clerk user ID from token (sub claim)
        var clerkUserId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(clerkUserId))
        {
            throw new UnauthorizedAccessException("Missing Clerk user id (sub) in token.");
        }

        // Get other claims from token
        var email =
            User.FindFirst("email")?.Value ??
            User.FindFirst("email_address")?.Value ??
            string.Empty;

        var username =
            User.FindFirst("username")?.Value
            ?? (email.Contains("@") ? email.Split('@')[0] : $"user-{clerkUserId[..6]}");

        // Use service to get or create user
        return await _userService.GetOrCreateUserAsync(
            Guid.Empty,
            clerkUserId,
            email,
            username,
            DateTime.UtcNow,
            null,
            null
        );
    }
}