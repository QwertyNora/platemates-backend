using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Platemates.Application.Dtos;
using Platemates.Application.Interfaces;

namespace Platemates.Application.Services;

public class GooglePlacesService : IGooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GooglePlacesService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Google:PlacesApiKey"] ?? throw new InvalidOperationException("Google Places API Key is not configured");
    }

    public async Task<GooglePlacesSearchResultDto> SearchRestaurantsAsync(string query)
    {
        // Google Places Text Search API (stödjer types=restaurant)
        var url = "https://maps.googleapis.com/maps/api/place/textsearch/json" +
                  $"?query={Uri.EscapeDataString(query)}" +
                  "&type=restaurant" +
                  "&region=se" +
                  $"&key={_apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Google Places API returned {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Google API Response: {json}");

        var googleResponse = JsonSerializer.Deserialize<GoogleTextSearchResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (googleResponse?.Results == null)
        {
            return new GooglePlacesSearchResultDto(new List<GooglePlacePredictionDto>());
        }

        var predictions = googleResponse.Results
            .Select(r => new GooglePlacePredictionDto(
                r.PlaceId,
                r.Name,
                r.FormattedAddress
            ))
            .ToList();

        return new GooglePlacesSearchResultDto(predictions);
    }

    public async Task<GooglePlaceDetailsDto> GetPlaceDetailsAsync(string placeId)
    {
        // Google Places Details API endpoint
        var url = "https://maps.googleapis.com/maps/api/place/details/json" +
                  $"?place_id={Uri.EscapeDataString(placeId)}" +
                  "&fields=place_id,name,formatted_address,type,formatted_phone_number,website,geometry" +
                  $"&key={_apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Google Places API returned {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var googleResponse = JsonSerializer.Deserialize<GooglePlaceDetailsResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (googleResponse?.Result == null)
        {
            throw new InvalidOperationException("Place details not found");
        }

        var result = googleResponse.Result;

        // Extract cuisine type from types array (if available)
        string? cuisineType = null;
        if (result.Types != null && result.Types.Any())
        {
            // Look for restaurant type or cuisine type
            cuisineType = result.Types.FirstOrDefault(t =>
                t != "restaurant" &&
                t != "food" &&
                t != "point_of_interest" &&
                t != "establishment");
        }

        return new GooglePlaceDetailsDto(
            result.PlaceId,
            result.Name,
            result.FormattedAddress,
            cuisineType,
            result.FormattedPhoneNumber,
            result.Website,
            result.Geometry?.Location?.Lat,
            result.Geometry?.Location?.Lng
        );
    }

    // Text Search response models
    private class GoogleTextSearchResponse
    {
        public List<Result> Results { get; set; } = new();
    }

    private class Result
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; } = string.Empty;
    }

    private class Prediction
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("structured_formatting")]
        public StructuredFormatting? StructuredFormatting { get; set; }
    }

    private class StructuredFormatting
    {
        [JsonPropertyName("main_text")]
        public string MainText { get; set; } = string.Empty;

        [JsonPropertyName("secondary_text")]
        public string SecondaryText { get; set; } = string.Empty;
    }

    private class GooglePlaceDetailsResponse
    {
        public PlaceDetailsResult? Result { get; set; }
    }

    private class PlaceDetailsResult
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; } = string.Empty;

        public List<string>? Types { get; set; }

        [JsonPropertyName("formatted_phone_number")]
        public string? FormattedPhoneNumber { get; set; }

        public string? Website { get; set; }

        public PlaceGeometry? Geometry { get; set; }
    }

    private class PlaceGeometry
    {
        public PlaceLocation? Location { get; set; }
    }

    private class PlaceLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}