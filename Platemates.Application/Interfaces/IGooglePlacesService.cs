using Platemates.Application.Dtos;

namespace Platemates.Application.Interfaces;

public interface IGooglePlacesService
{
    /// <summary>
    /// Search for restaurant predictions using Google Places Autocomplete
    /// </summary>
    Task<GooglePlacesSearchResultDto> SearchRestaurantsAsync(string query);

    /// <summary>
    /// Get detailed information about a place using its place_id
    /// </summary>
    Task<GooglePlaceDetailsDto> GetPlaceDetailsAsync(string placeId);
}