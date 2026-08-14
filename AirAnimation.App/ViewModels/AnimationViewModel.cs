using CommunityToolkit.Mvvm.ComponentModel;

namespace AirAnimation.App.ViewModels;

public sealed partial class AnimationViewModel : ObservableObject
{
    [ObservableProperty] private double speedMultiplier = 1.0;
    [ObservableProperty] private bool cameraFollow = true;
    [ObservableProperty] private double cameraPitch = 50;
    [ObservableProperty] private double cameraZoom = 7.5; // Altitude / Zoom level (3..15)
    [ObservableProperty] private double cameraBearingOffset = 0; // -180..+180
    [ObservableProperty] private string cameraMode = "follow"; // 'follow', 'overview', 'isometric'
    
    // Heading / Flight Orientation Mode:
    // 'forward' (Носом по курсу 0°), 'isometric' (Ракурс 3/4 +45°), 'side' (Сбоку +90°), 'custom'
    [ObservableProperty] private string orientationMode = "forward";
    [ObservableProperty] private double modelAngleOffset = 0; // -180..+180
    [ObservableProperty] private double flightAltitude = 2500; // 3D Altitude in meters (500..10000)
    [ObservableProperty] private bool enableBanking = true; // 3D aircraft roll on turns

    [ObservableProperty] private bool enableSpaceIntro = false; // Globe/Space intro animation

    // Route Drawing Mode: 'trailOnly' (TravelBoast style) vs 'fullRoute' (Classic preview)
    [ObservableProperty] private string routeDrawMode = "trailOnly"; 
    // Trail Style: 'redDashed', 'whiteDashed', 'neon', 'solid'
    [ObservableProperty] private string trailStyle = "redDashed";

    // Map Style: 'dark', 'light', 'satellite', 'cartoon'
    [ObservableProperty] private string mapStyle = "dark";

    [ObservableProperty] private bool showTrail = true;
    [ObservableProperty] private bool showCityLabels = true;
    [ObservableProperty] private bool showCountryFlags = true;
    [ObservableProperty] private double progress;          // 0..1
    [ObservableProperty] private double totalDistanceKm;
    [ObservableProperty] private bool isPlaying;

    public event EventHandler<double>? SpeedChanged;
    public event EventHandler<CameraSettingsEventArgs>? CameraChanged;
    public event EventHandler<RouteSettingsEventArgs>? RouteSettingsChanged;
    public event EventHandler<OrientationSettingsEventArgs>? OrientationSettingsChanged;
    public event EventHandler<string>? MapStyleChanged;

    public string SpeedLabel => $"{SpeedMultiplier:F1}×";

    public string DistanceLabel => TotalDistanceKm > 0
        ? $"{TotalDistanceKm:F1} км"
        : "—";

    partial void OnSpeedMultiplierChanged(double value)
    {
        OnPropertyChanged(nameof(SpeedLabel));
        SpeedChanged?.Invoke(this, value);
    }

    partial void OnCameraFollowChanged(bool value) => RaiseCameraUpdate();
    partial void OnCameraPitchChanged(double value) => RaiseCameraUpdate();
    partial void OnCameraZoomChanged(double value) => RaiseCameraUpdate();
    partial void OnCameraBearingOffsetChanged(double value) => RaiseCameraUpdate();
    partial void OnCameraModeChanged(string value) => RaiseCameraUpdate();

    partial void OnOrientationModeChanged(string value)
    {
        ModelAngleOffset = value switch
        {
            "isometric" => 45,
            "side"      => 90,
            _           => 0
        };
        RaiseOrientationUpdate();
    }

    partial void OnModelAngleOffsetChanged(double value) => RaiseOrientationUpdate();
    partial void OnFlightAltitudeChanged(double value) => RaiseOrientationUpdate();
    partial void OnEnableBankingChanged(bool value) => RaiseOrientationUpdate();

    partial void OnRouteDrawModeChanged(string value) => RaiseRouteSettingsUpdate();
    partial void OnTrailStyleChanged(string value) => RaiseRouteSettingsUpdate();
    
    partial void OnMapStyleChanged(string value) => MapStyleChanged?.Invoke(this, value);

    partial void OnTotalDistanceKmChanged(double value) =>
        OnPropertyChanged(nameof(DistanceLabel));

    private void RaiseCameraUpdate()
    {
        CameraChanged?.Invoke(this, new CameraSettingsEventArgs(
            CameraFollow, CameraPitch, CameraMode, CameraZoom, CameraBearingOffset));
    }

    private void RaiseRouteSettingsUpdate()
    {
        RouteSettingsChanged?.Invoke(this, new RouteSettingsEventArgs(RouteDrawMode, TrailStyle));
    }

    private void RaiseOrientationUpdate()
    {
        OrientationSettingsChanged?.Invoke(this, new OrientationSettingsEventArgs(
            OrientationMode, ModelAngleOffset, FlightAltitude, EnableBanking));
    }
}

public record CameraSettingsEventArgs(
    bool Follow,
    double Pitch,
    string Mode,
    double Zoom,
    double BearingOffset
);

public record RouteSettingsEventArgs(
    string DrawMode,
    string TrailStyle
);

public record OrientationSettingsEventArgs(
    string Mode,
    double AngleOffset,
    double Altitude,
    bool Banking
);
