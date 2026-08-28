using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AirAnimation.App.Models;
using AirAnimation.App.Services;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;

namespace AirAnimation.App.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly RouteService _routeService = new();
    private readonly GeocodingService _geocodingService = new();
    private string? _currentFilePath;

    [ObservableProperty] private MapViewModel mapViewModel;
    [ObservableProperty] private RouteViewModel routeViewModel;
    [ObservableProperty] private TransportViewModel transportViewModel;
    [ObservableProperty] private AnimationViewModel animationViewModel;
    [ObservableProperty] private ExportViewModel exportViewModel;

    [ObservableProperty] private string appTitle = "AirAnimation";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = "Готово";
    [ObservableProperty] private int selectedTabIndex;

    public MainViewModel()
    {
        MapViewModel        = new MapViewModel();
        RouteViewModel      = new RouteViewModel(_routeService, _geocodingService);
        TransportViewModel  = new TransportViewModel();
        AnimationViewModel  = new AnimationViewModel();
        ExportViewModel     = new ExportViewModel();
        ExportViewModel.Initialize(MapViewModel, RouteViewModel, AnimationViewModel);

        // Wire up cross-VM events
        TransportViewModel.TransportChanged += OnTransportChanged;
        TransportViewModel.TransportSizeChanged += OnTransportSizeChanged;
        RouteViewModel.RouteUpdated += OnRouteUpdated;
        RouteViewModel.BoundsRequested += async (_, _) => await MapViewModel.FitBoundsAsync();
    }

    private async void OnTransportChanged(object? sender, TransportModel transport)
    {
        await MapViewModel.SetTransportAsync(transport, TransportViewModel.TransportSize);
        StatusMessage = $"Транспорт: {transport.Name}";
    }

    private async void OnTransportSizeChanged(object? sender, double size)
    {
        await MapViewModel.SetTransportSizeAsync(size);
    }

    private async void OnRouteUpdated(object? sender, EventArgs e)
    {
        var allCoords = RouteViewModel.GetAllRouteCoordinates();
        if (allCoords.Count >= 2)
        {
            await MapViewModel.SetRouteAsync(allCoords, "#4F6BFF", 4);
            AnimationViewModel.TotalDistanceKm = RouteViewModel.TotalDistanceKm;
        }
        else
        {
            await MapViewModel.ClearRouteAsync();
            AnimationViewModel.TotalDistanceKm = 0;
            AnimationViewModel.Progress = 0;
        }
    }

    [RelayCommand]
    private async Task NewProjectAsync()
    {
        RouteViewModel.ClearAll();
        await MapViewModel.ClearAllAsync();
        _currentFilePath = null;
        AppTitle = "AirAnimation — Новый проект";
        StatusMessage = "Новый проект создан";
    }

    [RelayCommand]
    private async Task OpenProjectAsync()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "AirAnimation Route (*.airroute)|*.airroute",
            InitialDirectory = ProjectService.ProjectsDirectory
        };
        if (dlg.ShowDialog() != true) return;

        IsBusy = true;
        StatusMessage = "Загрузка...";
        try
        {
            var route = await ProjectService.LoadAsync(dlg.FileName);
            if (route is null) return;
            await LoadRouteAsync(route);
            _currentFilePath = dlg.FileName;
            AppTitle = $"AirAnimation — {route.Name}";
            StatusMessage = $"Загружено: {route.Name}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task SaveProjectAsync()
    {
        if (_currentFilePath is null) { await SaveAsProjectAsync(); return; }
        await SaveToFileAsync(_currentFilePath);
    }

    [RelayCommand]
    private async Task SaveAsProjectAsync()
    {
        var dlg = new SaveFileDialog
        {
            Filter = "AirAnimation Route (*.airroute)|*.airroute",
            DefaultExt = "airroute",
            InitialDirectory = ProjectService.ProjectsDirectory,
            FileName = RouteViewModel.RouteName
        };
        if (dlg.ShowDialog() != true) return;
        _currentFilePath = dlg.FileName;
        await SaveToFileAsync(_currentFilePath);
    }

    [RelayCommand]
    private async Task ImportGpxAsync()
    {
        var dlg = new OpenFileDialog { Filter = "GPX файлы (*.gpx)|*.gpx" };
        if (dlg.ShowDialog() != true) return;

        IsBusy = true;
        StatusMessage = "Импорт GPX...";
        try
        {
            var waypoints = GpxService.Parse(dlg.FileName);
            if (waypoints.Count == 0)
            {
                StatusMessage = "GPX файл не содержит точек";
                return;
            }
            await RouteViewModel.ImportWaypointsAsync(waypoints);
            await MapViewModel.FitBoundsAsync();
            StatusMessage = $"GPX импортирован: {waypoints.Count} точек";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка GPX: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task PlayAnimationAsync()
    {
        AnimationViewModel.IsPlaying = true;
        
        double? targetDuration = AnimationViewModel.UseTargetDuration ? AnimationViewModel.TargetDurationSeconds : null;
        
        await MapViewModel.PlayAsync(
            AnimationViewModel.SpeedMultiplier,
            AnimationViewModel.CameraFollow,
            AnimationViewModel.CameraPitch,
            AnimationViewModel.CameraMode,
            AnimationViewModel.CameraZoom,
            AnimationViewModel.CameraBearingOffset,
            AnimationViewModel.EnableSpaceIntro,
            targetDuration);
            
        StatusMessage = "Воспроизведение 3D анимации...";
    }

    [RelayCommand]
    private async Task PauseAnimationAsync()
    {
        AnimationViewModel.IsPlaying = false;
        await MapViewModel.PauseAsync();
        StatusMessage = "Пауза";
    }

    [RelayCommand]
    private async Task StopAnimationAsync()
    {
        AnimationViewModel.IsPlaying = false;
        await MapViewModel.StopAsync();
        AnimationViewModel.Progress = 0;
        StatusMessage = "Остановлено";
    }

    private async Task SaveToFileAsync(string path)
    {
        IsBusy = true;
        StatusMessage = "Сохранение...";
        try
        {
            var route = BuildRoute();
            await ProjectService.SaveAsync(route, path);
            StatusMessage = $"Сохранено: {Path.GetFileName(path)}";
        }
        finally { IsBusy = false; }
    }

    private Models.Route BuildRoute() => new()
    {
        Name = RouteViewModel.RouteName,
        Waypoints = RouteViewModel.Waypoints.Select((w, i) =>
            new Waypoint { Latitude = w.Lat, Longitude = w.Lon, Name = w.Name, Order = i }).ToList(),
        TransportId = TransportViewModel.SelectedTransport?.Id ?? "car",
        Animation = new AnimationSettings
        {
            SpeedMultiplier   = AnimationViewModel.SpeedMultiplier,
            CameraFollow      = AnimationViewModel.CameraFollow,
            CameraPitch       = AnimationViewModel.CameraPitch,
            ShowTrailLine     = AnimationViewModel.ShowTrail,
            TrailColor        = "#4F6BFF",
            ExportPreset      = ExportViewModel.SelectedPreset,
            VideoQuality      = ExportViewModel.SelectedQuality,
            Fps               = ExportViewModel.Fps,
        }
    };

    private async Task LoadRouteAsync(Models.Route route)
    {
        await RouteViewModel.ImportWaypointsAsync(route.Waypoints);
        TransportViewModel.SelectTransport(route.TransportId);
        AnimationViewModel.SpeedMultiplier = route.Animation.SpeedMultiplier;
        AnimationViewModel.CameraFollow    = route.Animation.CameraFollow;
    }
}
