namespace AirAnimation.App.Models;

public sealed class Route
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Мой маршрут";
    public string? Description { get; set; }
    public List<Waypoint> Waypoints { get; set; } = [];
    public List<RouteSegment> Segments { get; set; } = [];
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public string TransportId { get; set; } = "car";
    public AnimationSettings Animation { get; set; } = new();
    public MapSettings Map { get; set; } = new();
}

public sealed class RouteSegment
{
    public Guid FromWaypointId { get; set; }
    public Guid ToWaypointId { get; set; }
    /// <summary>Routed coordinates from OSRM.</summary>
    public List<double[]> Coordinates { get; set; } = [];
    public double DistanceMeters { get; set; }
    public double DurationSeconds { get; set; }
}

public sealed class AnimationSettings
{
    public double SpeedMultiplier { get; set; } = 1.0;
    public bool ShowCityLabels { get; set; } = true;
    public bool ShowCountryFlags { get; set; } = true;
    public bool CameraFollow { get; set; } = true;
    public double CameraZoom { get; set; } = 10;
    public double CameraPitch { get; set; } = 45;
    public bool ShowTrailLine { get; set; } = true;
    public string TrailColor { get; set; } = "#4F6BFF";
    public double TrailWidth { get; set; } = 4;
    public bool ShowDistanceCounter { get; set; } = true;
    public bool ShowSpeedometer { get; set; } = false;
    public ExportPreset ExportPreset { get; set; } = ExportPreset.YouTube;
    public VideoQuality VideoQuality { get; set; } = VideoQuality.HD1080;
    public int Fps { get; set; } = 30;
}

public sealed class MapSettings
{
    public MapStyle Style { get; set; } = MapStyle.Satellite;
    public bool Enable3D { get; set; } = true;
    public double InitialZoom { get; set; } = 5;
    public double InitialLat { get; set; } = 55.75;
    public double InitialLon { get; set; } = 37.62;
}

public enum MapStyle
{
    Streets,
    Satellite,
    Dark,
    Light,
    Outdoors,
    Topo
}

public enum ExportPreset
{
    TikTok,       // 9:16 1080x1920
    InstagramReels, // 9:16 1080x1920
    YouTubeShorts, // 9:16 1080x1920
    YouTube,      // 16:9 1920x1080
    Square        // 1:1  1080x1080
}

public enum VideoQuality
{
    HD720,
    HD1080,
    UHD4K
}
