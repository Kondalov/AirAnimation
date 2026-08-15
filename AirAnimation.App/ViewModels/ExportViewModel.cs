using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AirAnimation.App.Models;
using AirAnimation.App.Services;
using Microsoft.Win32;

namespace AirAnimation.App.ViewModels;

public sealed partial class ExportViewModel : ObservableObject
{
    private MapViewModel? _mapVm;
    private RouteViewModel? _routeVm;
    private AnimationViewModel? _animVm;

    [ObservableProperty] private ExportPreset selectedPreset = ExportPreset.YouTube;
    [ObservableProperty] private VideoQuality selectedQuality = VideoQuality.HD1080;
    [ObservableProperty] private int fps = 30;
    [ObservableProperty] private int videoDurationSeconds = 12; // 12 seconds like TravelBoast
    [ObservableProperty] private bool includeHud = false;
    [ObservableProperty] private bool isExporting;
    [ObservableProperty] private double exportProgress;
    [ObservableProperty] private string exportStatus = string.Empty;

    public ObservableCollection<ExportPreset> Presets { get; } =
        [.. Enum.GetValues<ExportPreset>()];

    public ObservableCollection<VideoQuality> Qualities { get; } =
        [.. Enum.GetValues<VideoQuality>()];

    public ObservableCollection<int> FpsOptions { get; } = [24, 30, 60];

    public string PresetDescription => SelectedPreset switch
    {
        ExportPreset.TikTok or ExportPreset.InstagramReels or ExportPreset.YouTubeShorts
            => "9:16 · 1080×1920",
        ExportPreset.YouTube => "16:9 · 1920×1080",
        ExportPreset.Square  => "1:1 · 1080×1080",
        _ => string.Empty
    };

    public void Initialize(MapViewModel mapVm, RouteViewModel routeVm, AnimationViewModel animVm)
    {
        _mapVm = mapVm;
        _routeVm = routeVm;
        _animVm = animVm;
    }

    partial void OnSelectedPresetChanged(ExportPreset value) =>
        OnPropertyChanged(nameof(PresetDescription));

    [RelayCommand]
    private async Task ExportVideoAsync()
    {
        if (_routeVm == null || _routeVm.Waypoints.Count < 2)
        {
            ExportStatus = "⚠️ Сначала добавьте минимум 2 точки маршрута на карту!";
            return;
        }

        var dlg = new SaveFileDialog
        {
            Filter = "MP4 Video (*.mp4)|*.mp4",
            DefaultExt = "mp4",
            FileName = $"AirAnimation_{DateTime.Now:yyyyMMdd_HHmmss}.mp4"
        };
        if (dlg.ShowDialog() != true) return;

        var framesDir = Path.Combine(Path.GetTempPath(), "AirAnimation_frames_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(framesDir);

        IsExporting = true;
        ExportProgress = 0;
        ExportStatus = "Подготовка к захвату кадров...";

        try
        {
            // Smart Video Duration logic
            double durationSec = 12; // default
            if (_animVm != null && _routeVm != null)
            {
                if (_animVm.UseTargetDuration)
                {
                    durationSec = _animVm.TargetDurationSeconds;
                }
                else
                {
                    // Calculate based on speed multiplier
                    double speedKmh = 100; // default transport speed, could be read from TransportViewModel but we don't have it here
                    double realHours = _routeVm.TotalDistanceKm / speedKmh;
                    durationSec = (realHours * 3600) / Math.Max(0.1, _animVm.SpeedMultiplier);
                }
            }
            
            // Limit duration strictly between 5 and 60 seconds
            durationSec = Math.Max(5, Math.Min(60, durationSec));
            
            int totalFrames = (int)Math.Max(30, durationSec * Fps);

            if (_mapVm != null)
            {
                await _mapVm.SetExportModeAsync(true);
                await _mapVm.SetHudVisibilityAsync(IncludeHud);
            }

            // Step 1: Deterministic Frame Capture
            for (int i = 0; i < totalFrames; i++)
            {
                double p = (double)i / (totalFrames - 1);
                
                if (_mapVm != null)
                {
                    double dtMs = 1000.0 / Fps;
                    await _mapVm.SeekAsync(p, dtMs);
                    await Task.Delay(25); // allow canvas render

                    var framePath = Path.Combine(framesDir, $"frame_{i:D4}.png");
                    await using (var fs = File.Create(framePath))
                    {
                        await _mapVm.CapturePreviewAsync(fs);
                    }
                }

                ExportProgress = (double)i / (totalFrames * 1.4);
                ExportStatus = $"Захват кадров: {i + 1}/{totalFrames} ({ExportProgress:P0})...";
            }

            // Step 2: FFmpeg Encoding
            ExportStatus = "Кодирование MP4 через встроенный FFmpeg...";
            VideoExportService.Configure();

            var opts = new VideoExportService.ExportOptions(
                framesDir, dlg.FileName, SelectedPreset, SelectedQuality, Fps);

            var prog = new Progress<double>(p =>
            {
                ExportProgress = 0.70 + (p * 0.30);
                ExportStatus = $"Сборка видео: {ExportProgress:P0}";
            });

            bool ok = await VideoExportService.AssembleAsync(opts, prog);
            ExportStatus = ok
                ? $"✅ Готово! Видео сохранено:\n{dlg.FileName}"
                : "❌ Ошибка при сборке MP4 видео";

            ExportProgress = 1.0;
        }
        catch (Exception ex)
        {
            ExportStatus = $"Ошибка экспорта: {ex.Message}";
        }
        finally
        {
            IsExporting = false;
            if (_mapVm != null)
                await _mapVm.SetExportModeAsync(false);

            // Clean up temporary frame folder
            if (Directory.Exists(framesDir))
            {
                try { Directory.Delete(framesDir, recursive: true); } catch { }
            }
        }
    }
}
