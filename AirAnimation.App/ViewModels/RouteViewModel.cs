using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AirAnimation.App.Models;
using AirAnimation.App.Services;

namespace AirAnimation.App.ViewModels;

public sealed class WaypointItemViewModel : ObservableObject
{
    public string Id { get; } = Guid.NewGuid().ToString();

    private string? _name;
    public string? Name { get => _name; set => SetProperty(ref _name, value); }

    private double _lat;
    public double Lat { get => _lat; set => SetProperty(ref _lat, value); }

    private double _lon;
    public double Lon { get => _lon; set => SetProperty(ref _lon, value); }

    public string DisplayName => Name ?? $"{Lat:F4}°, {Lon:F4}°";
    public string CoordText => $"{Lat:F4}°N  {Lon:F4}°E";
}

public sealed partial class RouteViewModel : ObservableObject
{
    private readonly RouteService _routeService;

    public ObservableCollection<WaypointItemViewModel> Waypoints { get; } = [];
    public List<RouteSegment> Segments { get; private set; } = [];

    [ObservableProperty] private string routeName = "Мой маршрут";
    [ObservableProperty] private double totalDistanceKm;
    [ObservableProperty] private WaypointItemViewModel? selectedWaypoint;
    [ObservableProperty] private bool isRouting;

    public event EventHandler? RouteUpdated;

    // Raised to tell MapView to add/remove a marker
    public event EventHandler<(string id, double lat, double lon, string? label)>? WaypointAdded;
    public event EventHandler<string>? WaypointRemoved;

    public RouteViewModel(RouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task AddWaypointFromMapAsync(double lat, double lon)
    {
        var item = new WaypointItemViewModel { Lat = lat, Lon = lon };
        Waypoints.Add(item);
        WaypointAdded?.Invoke(this, (item.Id, lat, lon, item.Name));

        if (Waypoints.Count >= 2)
            await RefreshSegmentsAsync();
    }

    [RelayCommand]
    private async Task RemoveWaypointAsync(WaypointItemViewModel? item)
    {
        if (item is null) return;
        Waypoints.Remove(item);
        WaypointRemoved?.Invoke(this, item.Id);

        if (Waypoints.Count >= 2)
            await RefreshSegmentsAsync();
        else
        {
            Segments.Clear();
            TotalDistanceKm = 0;
            RouteUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private async Task MoveWaypointUpAsync(WaypointItemViewModel? item)
    {
        if (item is null) return;
        var idx = Waypoints.IndexOf(item);
        if (idx <= 0) return;
        Waypoints.Move(idx, idx - 1);
        await RefreshSegmentsAsync();
    }

    [RelayCommand]
    private async Task MoveWaypointDownAsync(WaypointItemViewModel? item)
    {
        if (item is null) return;
        var idx = Waypoints.IndexOf(item);
        if (idx < 0 || idx >= Waypoints.Count - 1) return;
        Waypoints.Move(idx, idx + 1);
        await RefreshSegmentsAsync();
    }

    public void UpdateWaypointPosition(string id, double lat, double lon)
    {
        var item = Waypoints.FirstOrDefault(w => w.Id == id);
        if (item is null) return;
        item.Lat = lat;
        item.Lon = lon;
        _ = RefreshSegmentsAsync();
    }

    public async Task ImportWaypointsAsync(IEnumerable<Waypoint> waypoints)
    {
        Waypoints.Clear();
        Segments.Clear();
        foreach (var wp in waypoints.OrderBy(w => w.Order))
        {
            var item = new WaypointItemViewModel
            {
                Lat  = wp.Latitude,
                Lon  = wp.Longitude,
                Name = wp.Name
            };
            Waypoints.Add(item);
            WaypointAdded?.Invoke(this, (item.Id, item.Lat, item.Lon, item.Name));
        }
        if (Waypoints.Count >= 2)
            await RefreshSegmentsAsync();
    }

    public void ClearAll()
    {
        foreach (var w in Waypoints.ToList())
            WaypointRemoved?.Invoke(this, w.Id);
        Waypoints.Clear();
        Segments.Clear();
        TotalDistanceKm = 0;
    }

    public IReadOnlyList<double[]> GetAllRouteCoordinates()
    {
        if (Segments.Count == 0) return [];
        var result = new List<double[]>();
        foreach (var seg in Segments)
        {
            result.AddRange(seg.Coordinates);
        }
        return result;
    }

    private async Task RefreshSegmentsAsync()
    {
        if (Waypoints.Count < 2) return;
        IsRouting = true;
        try
        {
            var segments = new List<RouteSegment>();
            double totalDist = 0;
            for (int i = 0; i < Waypoints.Count - 1; i++)
            {
                var from = new Waypoint { Latitude = Waypoints[i].Lat, Longitude = Waypoints[i].Lon, Id = Guid.Parse(Waypoints[i].Id) };
                var to   = new Waypoint { Latitude = Waypoints[i+1].Lat, Longitude = Waypoints[i+1].Lon, Id = Guid.Parse(Waypoints[i+1].Id) };
                var seg  = await _routeService.GetSegmentAsync(from, to, "driving");
                segments.Add(seg);
                totalDist += seg.DistanceMeters;
            }
            Segments = segments;
            TotalDistanceKm = Math.Round(totalDist / 1000, 1);
            RouteUpdated?.Invoke(this, EventArgs.Empty);
        }
        finally { IsRouting = false; }
    }
}
