using AirAnimation.App.Services;
using System.Windows;

namespace AirAnimation.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Configure FFmpeg path (bundled)
        VideoExportService.Configure();
    }
}
