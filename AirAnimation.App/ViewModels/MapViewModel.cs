using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using AirAnimation.App.Services;
using AirAnimation.App.Models;
using Microsoft.Web.WebView2.Wpf;

namespace AirAnimation.App.ViewModels;

/// <summary>Mediates between the WPF MapView and the JS bridge.</summary>
public sealed partial class MapViewModel : ObservableObject
{
    private readonly MapBridgeService _bridge = new();

    [ObservableProperty] private bool isMapReady;
    [ObservableProperty] private string currentStyleKey = "satellite";

    public MapBridgeService Bridge => _bridge;

    public void OnMapReady()
    {
        IsMapReady = true;
    }

    public void AttachWebView(WebView2 wv) => _bridge.Attach(wv);

    public async Task SetTransportAsync(TransportModel transport, double size = 84)
    {
        await Bridge.SetTransportAsync(transport.Id, transport.SvgIcon, size, transport.DefaultSpeed);
    }

    public Task SetTransportSizeAsync(double size) =>
        _bridge.SetTransportSizeAsync(size);

    public Task SetRouteAsync(IReadOnlyList<double[]> coords, string color, double width) =>
        _bridge.SetRouteAsync(coords, color, width);

    public Task ClearAllAsync() => Task.WhenAll(
        _bridge.ClearWaypointsAsync(),
        _bridge.ClearRouteAsync());

    public Task FitBoundsAsync() => _bridge.FitBoundsAsync();

    public Task AddWaypointAsync(string id, double lat, double lon, string? label, int index) =>
        _bridge.AddWaypointAsync(id, lat, lon, label ?? string.Empty, index);

    public Task RemoveWaypointAsync(string id) => _bridge.RemoveWaypointAsync(id);

    public Task ReorderWaypointsAsync(IEnumerable<string> ids) =>
        _bridge.ReorderWaypointsAsync(ids);

    public Task PlayAsync(double speed, bool follow, double pitch, string mode = "follow", double zoom = 7.5, double bearingOffset = 0, bool globeIntro = false, double? targetDurationSeconds = null) =>
        _bridge.PlayAnimationAsync(speed, follow, pitch, mode, zoom, bearingOffset, globeIntro, targetDurationSeconds);

    public Task PauseAsync() => _bridge.PauseAnimationAsync();

    public Task StopAsync() => _bridge.StopAnimationAsync();

    public Task SetSpeedAsync(double speed) => _bridge.SetSpeedAsync(speed);

    public Task SetCameraSettingsAsync(bool follow, double pitch, string mode = "follow", double zoom = 7.5, double bearingOffset = 0) =>
        _bridge.SetCameraSettingsAsync(follow, pitch, mode, zoom, bearingOffset);

    public Task SetRouteSettingsAsync(string drawMode, string trailStyle) =>
        _bridge.SetRouteSettingsAsync(drawMode, trailStyle);

    public Task SetOrientationSettingsAsync(string mode, double angleOffset, double altitude, bool banking, bool smoothAnimation = false) =>
        _bridge.SetOrientationSettingsAsync(mode, angleOffset, altitude, banking, smoothAnimation);

    public Task SeekAsync(double progress, double? dtMs = null) => _bridge.SeekAnimationAsync(progress, dtMs);

    public Task SetExportModeAsync(bool isExporting) => _bridge.SetExportModeAsync(isExporting);

    public event EventHandler<(int Width, int Height)>? ViewportSizeChanged;
    public void SetViewportSize(int width, int height) => ViewportSizeChanged?.Invoke(this, (width, height));
    public void ResetViewportSize() => ViewportSizeChanged?.Invoke(this, (-1, -1));

    public Task SetSnowIntensityAsync(double intensity) => _bridge.SetSnowIntensityAsync(intensity);
    
    public Task SetCityPopupsAsync(bool show) => _bridge.SetCityPopupsAsync(show);

    public Task SetHudVisibilityAsync(bool visible) => _bridge.SetHudVisibilityAsync(visible);

    public Task CapturePreviewAsync(Stream stream) => _bridge.CapturePreviewAsync(stream);

    public async Task SetMapStyleAsync(string styleKey)
    {
        CurrentStyleKey = styleKey;
        await _bridge.SetMapStyleAsync(styleKey);
    }
}
