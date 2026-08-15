using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace AirAnimation.App.Services;

/// <summary>
/// Bi-directional bridge between C# (WPF) and JavaScript (MapLibre in WebView2).
/// </summary>
public sealed class MapBridgeService
{
    private WebView2? _webView;
    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    // ── Events raised from JS ─────────────────────────────────────────────────
    public event EventHandler<MapClickArgs>? MapClicked;
    public event EventHandler<WaypointMovedArgs>? WaypointMoved;
    public event EventHandler? MapReady;
    public event EventHandler<double>? AnimationProgressChanged;

    public void Attach(WebView2 webView)
    {
        _webView = webView;
        _webView.WebMessageReceived += OnWebMessageReceived;
    }

    // ── Messages from JS ──────────────────────────────────────────────────────
    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            var raw = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(raw)) return;

            using var doc = JsonDocument.Parse(raw);
            var type = doc.RootElement.GetProperty("type").GetString();

            switch (type)
            {
                case "mapReady":
                    MapReady?.Invoke(this, EventArgs.Empty);
                    break;

                case "mapClick":
                    var lat = doc.RootElement.GetProperty("lat").GetDouble();
                    var lon = doc.RootElement.GetProperty("lon").GetDouble();
                    MapClicked?.Invoke(this, new MapClickArgs(lat, lon));
                    break;

                case "waypointMoved":
                    var id   = doc.RootElement.GetProperty("id").GetString()!;
                    var mlat = doc.RootElement.GetProperty("lat").GetDouble();
                    var mlon = doc.RootElement.GetProperty("lon").GetDouble();
                    WaypointMoved?.Invoke(this, new WaypointMovedArgs(id, mlat, mlon));
                    break;

                case "animationProgress":
                    var prog = doc.RootElement.GetProperty("progress").GetDouble();
                    AnimationProgressChanged?.Invoke(this, prog);
                    break;
            }
        }
        catch { /* ignore parse errors */ }
    }

    // ── Commands to JS ────────────────────────────────────────────────────────
    public Task AddWaypointAsync(string id, double lat, double lon, string label) =>
        ExecAsync("addWaypoint", new { id, lat, lon, label });

    public Task MoveWaypointAsync(string id, double lat, double lon) =>
        ExecAsync("moveWaypoint", new { id, lat, lon });

    public Task RemoveWaypointAsync(string id) =>
        ExecAsync("removeWaypoint", new { id });

    public Task ClearWaypointsAsync() =>
        ExecAsync("clearWaypoints", new { });

    public Task SetRouteAsync(IEnumerable<double[]> coordinates, string color, double width) =>
        ExecAsync("setRoute", new { coordinates, color, width });

    public Task ClearRouteAsync() =>
        ExecAsync("clearRoute", new { });

    public Task SetTransportAsync(string svgContent, double size = 64, double speedKmh = 300) =>
        ExecAsync("setTransport", new { svg = svgContent, size, speedKmh });

    public Task SetTransportSizeAsync(double size) =>
        ExecAsync("setTransportSize", new { size });

    public Task SetRouteSettingsAsync(string drawMode, string trailStyle) =>
        ExecAsync("setRouteSettings", new { drawMode, trailStyle });

    public Task SetMapStyleAsync(string styleUrl) =>
        ExecAsync("setMapStyle", new { styleUrl });

    public Task PlayAnimationAsync(double speedMultiplier, bool cameraFollow, double cameraPitch, string cameraMode = "follow", double cameraZoom = 7.5, double cameraBearingOffset = 0, bool globeIntro = false, double? targetDurationSeconds = null) =>
        ExecAsync("playAnimation", new { speedMultiplier, cameraFollow, cameraPitch, cameraMode, cameraZoom, cameraBearingOffset, globeIntro, targetDurationSeconds });

    public Task PauseAnimationAsync() =>
        ExecAsync("pauseAnimation", new { });

    public Task StopAnimationAsync() =>
        ExecAsync("stopAnimation", new { });

    public Task SetSpeedAsync(double speedMultiplier) =>
        ExecAsync("setSpeed", new { speedMultiplier });

    public Task SetCameraSettingsAsync(bool cameraFollow, double cameraPitch, string cameraMode = "follow", double cameraZoom = 7.5, double cameraBearingOffset = 0) =>
        ExecAsync("setCameraSettings", new { cameraFollow, cameraPitch, cameraMode, cameraZoom, cameraBearingOffset });

    public Task SetOrientationSettingsAsync(string mode, double angleOffset, double altitude, bool banking, bool smoothAnimation = false) =>
        ExecAsync("setOrientationSettings", new { mode, angleOffset, altitude, banking, smoothAnimation });

    public Task SeekAnimationAsync(double progress, double? dtMs = null) =>
        ExecAsync("seekAnimation", new { progress, dtMs });

    public Task SetExportModeAsync(bool isExporting) =>
        ExecAsync("setExportMode", new { isExporting });

    public Task SetHudVisibilityAsync(bool visible) =>
        ExecAsync("setHudVisibility", new { visible });

    public Task SetCloudOpacityAsync(double opacity) =>
        ExecAsync("setCloudOpacity", new { opacity });

    public Task SetRainIntensityAsync(double intensity) =>
        ExecAsync("setRainIntensity", new { intensity });

    public Task SetSnowIntensityAsync(double intensity) =>
        ExecAsync("setSnowIntensity", new { intensity });

    public Task FitBoundsAsync() =>
        ExecAsync("fitBounds", new { });

    public async Task CapturePreviewAsync(Stream stream)
    {
        if (_webView?.CoreWebView2 is { } core)
        {
            await core.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, stream);
        }
    }

    public Task<string> CaptureScreenshotAsync() =>
        EvalAsync("captureScreenshot()");

    private async Task ExecAsync(string fn, object args)
    {
        if (_webView is null) return;
        var json = JsonSerializer.Serialize(args, _json);
        var script = $"window.bridge.{fn}({json});";
        await _webView.ExecuteScriptAsync(script);
    }

    private async Task<string> EvalAsync(string expr)
    {
        if (_webView is null) return string.Empty;
        return await _webView.ExecuteScriptAsync(expr);
    }
}

public record MapClickArgs(double Lat, double Lon);
public record WaypointMovedArgs(string Id, double Lat, double Lon);
