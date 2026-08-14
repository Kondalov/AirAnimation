using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using AirAnimation.App.Services;
using Microsoft.Web.WebView2.Wpf;

namespace AirAnimation.App.ViewModels;

/// <summary>Mediates between the WPF MapView and the JS bridge.</summary>
public sealed partial class MapViewModel : ObservableObject
{
    private readonly MapBridgeService _bridge = new();

    [ObservableProperty] private bool isMapReady;
    [ObservableProperty] private string currentStyleKey = "dark";

    public MapBridgeService Bridge => _bridge;

    public void OnMapReady()
    {
        IsMapReady = true;
    }

    public void AttachWebView(WebView2 wv) => _bridge.Attach(wv);

    public Task SetTransportAsync(string svgContent, double size = 64) =>
        _bridge.SetTransportAsync(svgContent, size);

    public Task SetTransportSizeAsync(double size) =>
        _bridge.SetTransportSizeAsync(size);

    public Task SetRouteAsync(IReadOnlyList<double[]> coords, string color, double width) =>
        _bridge.SetRouteAsync(coords, color, width);

    public Task ClearAllAsync() => Task.WhenAll(
        _bridge.ClearWaypointsAsync(),
        _bridge.ClearRouteAsync());

    public Task FitBoundsAsync() => _bridge.FitBoundsAsync();

    public Task AddWaypointAsync(string id, double lat, double lon, string? label) =>
        _bridge.AddWaypointAsync(id, lat, lon, label ?? string.Empty);

    public Task RemoveWaypointAsync(string id) => _bridge.RemoveWaypointAsync(id);

    public Task PlayAsync(double speed, bool follow, double pitch, string mode = "follow", double zoom = 7.5, double bearingOffset = 0) =>
        _bridge.PlayAnimationAsync(speed, follow, pitch, mode, zoom, bearingOffset);

    public Task PauseAsync() => _bridge.PauseAnimationAsync();

    public Task StopAsync() => _bridge.StopAnimationAsync();

    public Task SetSpeedAsync(double speed) => _bridge.SetSpeedAsync(speed);

    public Task SetCameraSettingsAsync(bool follow, double pitch, string mode = "follow", double zoom = 7.5, double bearingOffset = 0) =>
        _bridge.SetCameraSettingsAsync(follow, pitch, mode, zoom, bearingOffset);

    public Task SetRouteSettingsAsync(string drawMode, string trailStyle) =>
        _bridge.SetRouteSettingsAsync(drawMode, trailStyle);

    public Task SetOrientationSettingsAsync(string mode, double angleOffset, double altitude, bool banking) =>
        _bridge.SetOrientationSettingsAsync(mode, angleOffset, altitude, banking);

    public Task SeekAsync(double progress) => _bridge.SeekAnimationAsync(progress);

    public Task CapturePreviewAsync(Stream stream) => _bridge.CapturePreviewAsync(stream);

    public async Task SetMapStyleAsync(string styleKey)
    {
        CurrentStyleKey = styleKey;
        await _bridge.SetMapStyleAsync(styleKey);
    }
}
