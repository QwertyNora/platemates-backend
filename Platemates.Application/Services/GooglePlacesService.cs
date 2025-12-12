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
}