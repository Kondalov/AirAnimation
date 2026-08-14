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
        int Fps = 30);

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
        var pattern = Path.Combine(opts.FramesDirectory, "frame_%04d.png");

        var success = await FFMpegArguments
            .FromFileInput(pattern, verifyExists: false, o =>
            {
                o.WithFramerate(opts.Fps);
                o.ForceFormat("image2");
            })
            .OutputToFile(opts.OutputPath, overwrite: true, o =>
            {
                o.WithVideoCodec(VideoCodec.LibX264);
                o.WithConstantRateFactor(18);
                o.ForcePixelFormat("yuv420p");
                o.WithVideoBitrate(opts.Quality == VideoQuality.UHD4K ? 20000 : 8000);
                o.Resize(w, h);
                o.WithFramerate(opts.Fps);
                o.WithFastStart();
            })
            .CancellableThrough(ct)
            .ProcessAsynchronously();

        return success;
    }
}
