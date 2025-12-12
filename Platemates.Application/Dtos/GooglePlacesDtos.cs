namespace Platemates.Application.Dtos;

/// <summary>
/// Request DTO for searching restaurants via Google Places
/// </summary>
public record SearchRestaurantsDto(
    string Query
);

/// <summary>
/// Response DTO for a single Google Place autocomplete prediction
/// </summary>
public record GooglePlacePredictionDto(
    string PlaceId,
    string Name,
    string Address
);

/// <summary>
/// Response DTO for Google Places autocomplete results
/// </summary>
public record GooglePlacesSearchResultDto(
    List<GooglePlacePredictionDto> Predictions
);