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

    public RestaurantsController(IRestaurantService restaurantService, IUserService userService)
    {
        _restaurantService = restaurantService;
        _userService = userService;
    }

    /// <summary>
    /// Add a restaurant manually to "Want to Go" list
    /// </summary>
    [HttpPost("manual")]
    public async Task<ActionResult<UserRestaurantDto>> AddRestaurantManually(
        [FromBody] AddRestaurantManuallyDto dto,
        CancellationToken cancellationToken)
    {
        // Get Clerk user ID from token
        var clerkUserId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(clerkUserId))
        {
            return Unauthorized("Missing Clerk user id (sub) in token.");
        }

        // Get other claims from token
        var email =
            User.FindFirst("email")?.Value ??
            User.FindFirst("email_address")?.Value ??
            string.Empty;

        var username =
            User.FindFirst("username")?.Value
            ?? (email.Contains("@") ? email.Split('@')[0] : $"user-{clerkUserId[..6]}");

        // Get or create user
        var userDto = await _userService.GetOrCreateUserAsync(
            Guid.Empty,
            clerkUserId,
            email,
            username,
            DateTime.UtcNow,
            null,
            null
        );

        // Add restaurant
        var userRestaurant = await _restaurantService.AddRestaurantManuallyAsync(userDto.Id, dto);

        return CreatedAtAction(
            nameof(AddRestaurantManually),
            new { id = userRestaurant.Id },
            userRestaurant
        );
    }
}