using System.Windows;
using AirAnimation.App.ViewModels;

namespace AirAnimation.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        int useImmersiveDarkMode = 1;
        DwmSetWindowAttribute(hwnd, 20, ref useImmersiveDarkMode, sizeof(int));

        if (DataContext is MainViewModel vm)
        {
            // RouteViewModel → add/remove waypoints on map
            vm.RouteViewModel.WaypointAdded += async (_, args) =>
                await vm.MapViewModel.AddWaypointAsync(args.id, args.lat, args.lon, args.label, args.index);

            vm.RouteViewModel.WaypointRemoved += async (_, id) =>
                await vm.MapViewModel.RemoveWaypointAsync(id);

            vm.RouteViewModel.WaypointsReordered += async (_, ids) =>
                await vm.MapViewModel.ReorderWaypointsAsync(ids);

            // MapView click → RouteViewModel
            vm.MapViewModel.Bridge.MapClicked += async (_, args) =>
                await vm.RouteViewModel.AddWaypointFromMapAsync(args.Lat, args.Lon);

            vm.MapViewModel.Bridge.WaypointInserted += async (_, args) =>
                await vm.RouteViewModel.InsertWaypointFromMapAsync(args.Lat, args.Lon, args.Index);

            // Map drag → update position
            vm.MapViewModel.Bridge.WaypointMoved += (_, args) =>
                vm.RouteViewModel.UpdateWaypointPosition(args.Id, args.Lat, args.Lon);

            // Real-time animation speed update
            vm.AnimationViewModel.SpeedChanged += async (_, speed) =>
                await vm.MapViewModel.SetSpeedAsync(speed);

            // Real-time camera 3D settings update
            vm.AnimationViewModel.CameraChanged += async (_, cam) =>
                await vm.MapViewModel.SetCameraSettingsAsync(cam.Follow, cam.Pitch, cam.Mode, cam.Zoom, cam.BearingOffset);

            // Real-time route drawing mode & style update
            vm.AnimationViewModel.RouteSettingsChanged += async (_, r) =>
                await vm.MapViewModel.SetRouteSettingsAsync(r.DrawMode, r.TrailStyle);

            // Real-time 3D flight orientation update (Heading, Offset, Altitude, Banking)
            vm.AnimationViewModel.OrientationSettingsChanged += async (_, o) =>
                await vm.MapViewModel.SetOrientationSettingsAsync(o.Mode, o.AngleOffset, o.Altitude, o.Banking, o.SmoothAnimation);

            vm.AnimationViewModel.ShowCityPopupsChanged += async (_, show) =>
                await vm.MapViewModel.SetCityPopupsAsync(show);

            vm.AnimationViewModel.ShowCityLabelsChanged += async (_, _) =>
                await vm.MapViewModel.Bridge.SetMarkerVisibilityAsync(vm.AnimationViewModel.ShowCityLabels, vm.AnimationViewModel.ShowCountryFlags);

            vm.AnimationViewModel.ShowCountryFlagsChanged += async (_, _) =>
                await vm.MapViewModel.Bridge.SetMarkerVisibilityAsync(vm.AnimationViewModel.ShowCityLabels, vm.AnimationViewModel.ShowCountryFlags);

            // Map Style update
            vm.AnimationViewModel.MapStyleChanged += async (_, style) =>
                await vm.MapViewModel.SetMapStyleAsync(style);

            vm.AnimationViewModel.CloudOpacityChanged += async (_, opacity) =>
                await vm.MapViewModel.Bridge.SetCloudOpacityAsync(opacity);

            vm.AnimationViewModel.RainIntensityChanged += async (_, intensity) =>
                await vm.MapViewModel.Bridge.SetRainIntensityAsync(intensity);

            vm.AnimationViewModel.SnowIntensityChanged += async (_, intensity) =>
                await vm.MapViewModel.Bridge.SetSnowIntensityAsync(intensity);

            vm.AnimationViewModel.LightningIntensityChanged += async (_, intensity) =>
                await vm.MapViewModel.Bridge.SetLightningSettingsAsync(intensity, vm.AnimationViewModel.LightningSpeed);

            vm.AnimationViewModel.LightningSpeedChanged += async (_, speed) =>
                await vm.MapViewModel.Bridge.SetLightningSettingsAsync(vm.AnimationViewModel.LightningIntensity, speed);

            // Animation progress & completion → AnimationViewModel
            vm.MapViewModel.Bridge.AnimationProgressChanged += (_, p) =>
            {
                vm.AnimationViewModel.Progress = p;
                if (p >= 1.0)
                {
                    vm.AnimationViewModel.IsPlaying = false;
                }
            };

            vm.TransportViewModel.TransportSizeChanged += async (_, sz) =>
                await vm.MapViewModel.SetTransportSizeAsync(sz);

            vm.MapViewModel.Bridge.MapReady += (_, _) =>
            {
                vm.MapViewModel.OnMapReady();
                _ = vm.MapViewModel.Bridge.SetMarkerVisibilityAsync(vm.AnimationViewModel.ShowCityLabels, vm.AnimationViewModel.ShowCountryFlags);

                if (vm.TransportViewModel.SelectedTransport is { } t)
                    _ = vm.MapViewModel.SetTransportAsync(t.SvgIcon, vm.TransportViewModel.TransportSize, t.DefaultSpeed);
                else
                    vm.TransportViewModel.SelectTransport("airliner");
            };
        }
    }
}
