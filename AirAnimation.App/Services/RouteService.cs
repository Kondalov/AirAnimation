using System.Net.Http;
using System.Text.Json;
using AirAnimation.App.Models;

namespace AirAnimation.App.Services;

/// <summary>
/// Routes waypoints via the public OSRM API.
/// Falls back to straight-line interpolation if unavailable.
/// </summary>
public sealed class RouteService : IDisposable
{
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private const string OsrmBase = "https://router.project-osrm.org/route/v1";

    public async Task<RouteSegment> GetSegmentAsync(
        Waypoint from, Waypoint to, string profile, CancellationToken ct = default)
    {
        // profile: driving | walking | cycling | flight
        if (profile == "flight")
            return BuildArcPath(from, to);

        var url = $"{OsrmBase}/{profile}/{from.Longitude},{from.Latitude};{to.Longitude},{to.Latitude}?overview=full&geometries=geojson";

        try
        {
            var json = await _http.GetStringAsync(url, ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.GetProperty("code").GetString() != "Ok")
                return BuildArcPath(from, to);

            var route = root.GetProperty("routes")[0];
            var distance = route.GetProperty("distance").GetDouble();
            var duration = route.GetProperty("duration").GetDouble();
            var coords = route.GetProperty("geometry").GetProperty("coordinates");

            var segment = new RouteSegment
            {
                FromWaypointId = from.Id,
                ToWaypointId = to.Id,
                DistanceMeters = distance,
                DurationSeconds = duration
            };

            foreach (var coord in coords.EnumerateArray())
            {
                var lon = coord[0].GetDouble();
                var lat = coord[1].GetDouble();
                segment.Coordinates.Add([lon, lat]);
            }

            // Ensure route starts and ends EXACTLY at the user's waypoints
            if (segment.Coordinates.Count > 0)
            {
                var first = segment.Coordinates[0];
                if (Math.Abs(first[0] - from.Longitude) > 0.00001 || Math.Abs(first[1] - from.Latitude) > 0.00001)
                    segment.Coordinates.Insert(0, [from.Longitude, from.Latitude]);

                var last = segment.Coordinates[^1];
                if (Math.Abs(last[0] - to.Longitude) > 0.00001 || Math.Abs(last[1] - to.Latitude) > 0.00001)
                    segment.Coordinates.Add([to.Longitude, to.Latitude]);
            }
            else
            {
                segment.Coordinates.Add([from.Longitude, from.Latitude]);
                segment.Coordinates.Add([to.Longitude, to.Latitude]);
            }

            return segment;
        }
        catch
        {
            return BuildArcPath(from, to);
        }
    }

    /// <summary>Builds a smooth parabolic arc path between two points.</summary>
    private static RouteSegment BuildArcPath(Waypoint from, Waypoint to)
    {
        const int steps = 100;
        var coords = new List<double[]>(steps + 1);

        double dLat = (to.Latitude - from.Latitude) * Math.PI / 180;
        double dLon = (to.Longitude - from.Longitude) * Math.PI / 180;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                 + Math.Cos(from.Latitude * Math.PI / 180) * Math.Cos(to.Latitude * Math.PI / 180)
                 * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double dist = 6_371_000 * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Control point for quadratic bezier (curve upwards like TravelBoast)
        double distDegrees = Math.Sqrt(Math.Pow(to.Longitude - from.Longitude, 2) + Math.Pow(to.Latitude - from.Latitude, 2));
        double ctrlLon = (from.Longitude + to.Longitude) / 2;
        
        // Offset latitude upwards to create an arc
        // The longer the distance, the higher the arc. Cap the offset so it doesn't go crazy on very long flights.
        double offset = Math.Min(distDegrees * 0.25, 20.0);
        double ctrlLat = (from.Latitude + to.Latitude) / 2 + offset;

        for (int i = 0; i <= steps; i++)
        {
            double t = (double)i / steps;
            double u = 1 - t;
            
            // Quadratic Bezier formula: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
            double lon = u * u * from.Longitude + 2 * u * t * ctrlLon + t * t * to.Longitude;
            double lat = u * u * from.Latitude  + 2 * u * t * ctrlLat + t * t * to.Latitude;
            
            coords.Add([lon, lat]);
        }

        return new RouteSegment
        {
            FromWaypointId = from.Id,
            ToWaypointId = to.Id,
            Coordinates = coords,
            DistanceMeters = dist,
            DurationSeconds = dist / 250 // faster speed estimate for planes
        };
    }

    public static string ProfileForTransport(string transportId) => transportId switch
    {
        "bicycle" or "escooter" or "skateboard" => "cycling",
        "motorcycle" or "scooter" => "driving",
        _ when IsAirOrSea(transportId) => "driving", // OSRM doesn't support air; use straight
        _ => "driving"
    };

    private static bool IsAirOrSea(string id) =>
        id is "airliner" or "bizjet" or "helicopter" or "drone" or "glider" or "airship"
            or "hot_balloon" or "fighter" or "seaplane" or "rocket" or "shuttle" or "satellite"
            or "yacht" or "cruise" or "ferry" or "submarine" or "motorboat" or "kayak"
            or "ufo" or "magic_carpet" or "dragon" or "broomstick";

    public void Dispose() => _http.Dispose();
}
