using System.Text.Json.Serialization;

namespace AirAnimation.App.Models;

public sealed class Waypoint
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Name { get; set; }
    public string? PhotoPath { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; }
    public WaypointType Type { get; set; } = WaypointType.Stop;

    [JsonIgnore]
    public string DisplayName => Name ?? $"{Latitude:F4}°, {Longitude:F4}°";
}

public enum WaypointType
{
    Stop,
    Passthrough
}
