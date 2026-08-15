using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using AirAnimation.App.Models;

namespace AirAnimation.App.Services;

/// <summary>
/// Assembles captured PNG frames into an MP4 video via FFMpegCore.
/// </summary>
public sealed class VideoExportService
{
    /// <summary>Directory where ffmpeg.exe is located (bundled with app).</summary>
    public static string FfmpegDirectory =>
        Path.Combine(AppContext.BaseDirectory, "ffmpeg");

    public static void Configure() =>
        GlobalFFOptions.Configure(opts => opts.BinaryFolder = FfmpegDirectory);

    public record ExportOptions(
        string FramesDirectory,
        string OutputPath,
        ExportPreset Preset,
        VideoQuality Quality,
        int Fps = 30,
        bool EnableSounds = false,
        double RainIntensity = 0,
        double LightningIntensity = 0,
        double SnowIntensity = 0,
        double CloudOpacity = 0);

    public static (int Width, int Height) GetDimensions(ExportPreset preset, VideoQuality quality)
    {
        var (w, h) = preset switch
        {
            ExportPreset.TikTok or ExportPreset.InstagramReels or ExportPreset.YouTubeShorts
                => (1080, 1920),
            ExportPreset.YouTube
                => (1920, 1080),
            ExportPreset.Square
                => (1080, 1080),
            _ => (1920, 1080)
        };

        if (quality == VideoQuality.HD720)
        {
            w = w * 720 / 1080;
            h = h * 720 / 1080;
        }
        else if (quality == VideoQuality.UHD4K)
        {
            w *= 2;
            h *= 2;
        }

        return (w, h);
    }

    /// <summary>
    /// Converts a folder of sequentially-named PNG frames to MP4.
    /// Frame files should be named frame_0000.png, frame_0001.png, etc.
    /// </summary>
    public static async Task<bool> AssembleAsync(
        ExportOptions opts,
        IProgress<double>? progress = null,
        CancellationToken ct = default)
    {
        var (w, h) = GetDimensions(opts.Preset, opts.Quality);
        var pattern = Path.Combine(opts.FramesDirectory, "frame_%04d.jpg");
        var bitrate = opts.Quality == VideoQuality.UHD4K ? "20000k" : "8000k";
        var soundDir = Path.Combine(AppContext.BaseDirectory, "Resources", "Sound");

        var args = $"-y -framerate {opts.Fps} -f image2 -i \"{pattern}\" ";

        if (opts.EnableSounds)
        {
            args += $"-stream_loop -1 -i \"{Path.Combine(soundDir, "airplane.mp3")}\" ";
            args += $"-stream_loop -1 -i \"{Path.Combine(soundDir, "rain.mp3")}\" ";
            args += $"-stream_loop -1 -i \"{Path.Combine(soundDir, "wind.mp3")}\" ";

            var vRain = (opts.RainIntensity * 0.8).ToString(System.Globalization.CultureInfo.InvariantCulture);
            var vWind = Math.Min(1.0, (opts.SnowIntensity * 0.8) + (opts.CloudOpacity * 0.3)).ToString(System.Globalization.CultureInfo.InvariantCulture);

            args += $"-filter_complex \"[1:a]volume=1.0[a1];[2:a]volume={vRain}[a2];[3:a]volume={vWind}[a3];[a1][a2][a3]amix=inputs=3:duration=first:dropout_transition=0[aout]\" ";
            args += $"-map 0:v -map \"[aout]\" -c:v libx264 -preset ultrafast -crf 18 -pix_fmt yuv420p -c:a aac -b:v {bitrate} -s {w}x{h} -shortest \"{opts.OutputPath}\"";
        }
        else
        {
            args += $"-c:v libx264 -preset ultrafast -crf 18 -pix_fmt yuv420p -b:v {bitrate} -s {w}x{h} \"{opts.OutputPath}\"";
        }

        using var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(FfmpegDirectory, "ffmpeg.exe"),
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        ct.Register(() => { try { process.Kill(); } catch { } });
        process.Start();
        await process.WaitForExitAsync(ct);

        return process.ExitCode == 0;
    }
}
