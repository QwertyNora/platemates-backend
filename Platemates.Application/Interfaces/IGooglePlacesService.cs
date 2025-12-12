using Platemates.Application.Dtos;

namespace Platemates.Application.Interfaces;

public interface IGooglePlacesService
{
    /// <summary>
    /// Search for restaurant predictions using Google Places Autocomplete
    /// </summary>
    Task<GooglePlacesSearchResultDto> SearchRestaurantsAsync(string query);
}