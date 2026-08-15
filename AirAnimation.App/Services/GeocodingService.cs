using System.Net.Http;
using System.Text.Json;

namespace AirAnimation.App.Services;

/// <summary>
/// Geocodes location names into Lat/Lon coordinates using public OSM Nominatim.
/// </summary>
public sealed class GeocodingService : IDisposable
{
    private readonly HttpClient _http;

    public GeocodingService()
    {
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _http.DefaultRequestHeaders.Add("User-Agent", "AirAnimationApp/1.0 (https://github.com/KOLAN/AirAnimation)");
    }

    /// <summary>
    /// Searches for a location by name. Supports Russian and English.
    /// </summary>
    public async Task<(double Lat, double Lon, string Name)?> GeocodeAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return null;

        // Use accept-language=ru to prefer Russian localizations in the result display name
        var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=1&accept-language=ru,en";

        try
        {
            var json = await _http.GetStringAsync(url, ct);
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var element = doc.RootElement[0];
                var latStr = element.GetProperty("lat").GetString();
                var lonStr = element.GetProperty("lon").GetString();
                var fullName = element.GetProperty("display_name").GetString();

                if (double.TryParse(latStr, System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(lonStr, System.Globalization.CultureInfo.InvariantCulture, out var lon))
                {
                    // Nominatim returns detailed "City, Region, Country". Take the first meaningful part.
                    var shortName = fullName?.Split(',')[0].Trim() ?? query;
                    return (lat, lon, shortName);
                }
            }
        }
        catch 
        { 
            // Return null on network error, timeout, or parsing error
        }

        return null;
    }

    public void Dispose()
    {
        _http.Dispose();
    }
}
