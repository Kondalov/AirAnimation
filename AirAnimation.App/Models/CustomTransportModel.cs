namespace AirAnimation.App.Models;

/// <summary>
/// Represents a user-imported 3D model (GLB/GLTF) with metadata.
/// </summary>
public sealed class CustomTransportModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string VehicleType { get; set; } = "Авто";
    public string Emoji { get; set; } = "🚗";
    public double DefaultSpeed { get; set; } = 100;
    public bool FollowRoads { get; set; } = true;
    public double RotationOffsetX { get; set; } = 0;
    public double RotationOffsetY { get; set; } = 0;
    public double RotationOffsetZ { get; set; } = 0;
    public long FileSizeBytes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TransportModel ToTransportModel() => new()
    {
        Id = Id,
        Name = Name,
        Category = "Мои модели",
        Emoji = Emoji,
        SvgIcon = SvgIcons.Car,
        DefaultSpeed = DefaultSpeed,
        FollowRoads = FollowRoads,
        IsCustom = true,
        CustomModelFileName = FileName,
        RotationOffsetX = RotationOffsetX,
        RotationOffsetY = RotationOffsetY,
        RotationOffsetZ = RotationOffsetZ,
        Description = $"Пользовательская 3D модель ({VehicleType})"
    };
}
