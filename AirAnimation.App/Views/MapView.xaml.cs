using System.IO;
using System.Windows;
using System.Windows.Controls;
using AirAnimation.App.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace AirAnimation.App.Views;

public partial class MapView : UserControl
{
    public MapView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await WebMap.EnsureCoreWebView2Async();
        WebMap.CoreWebView2.Settings.IsStatusBarEnabled = false;
        WebMap.CoreWebView2.Settings.AreDevToolsEnabled = true; // dev only

        // Allow loading local resources + internet (CDN for MapLibre)
        WebMap.CoreWebView2.Settings.AreHostObjectsAllowed = true;
        
        // Map Virtual Host to Base Directory so JS can fetch() local 3D models without CORS errors
        WebMap.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "appassets.local", 
            AppContext.BaseDirectory, 
            CoreWebView2HostResourceAccessKind.Allow);

        // Navigate via virtual host so ES modules & importmap resolve correctly (file:// blocks ESM)
        WebMap.Source = new Uri("https://appassets.local/Resources/MapHtml/index.html");

        // Attach bridge
        if (DataContext is MapViewModel vm)
        {
            vm.AttachWebView(WebMap);
            vm.ViewportSizeChanged += (s, args) =>
            {
                if (args.Width > 0 && args.Height > 0)
                {
                    double targetRatio = (double)args.Width / args.Height;
                    double currentRatio = ActualWidth / ActualHeight;
                    
                    if (targetRatio > currentRatio)
                    {
                        // Constrain by width
                        WebMap.Width = ActualWidth;
                        WebMap.Height = ActualWidth / targetRatio;
                        WebMap.ZoomFactor = ActualWidth / args.Width;
                    }
                    else
                    {
                        // Constrain by height
                        WebMap.Height = ActualHeight;
                        WebMap.Width = ActualHeight * targetRatio;
                        WebMap.ZoomFactor = ActualHeight / args.Height;
                    }
                    WebMap.HorizontalAlignment = HorizontalAlignment.Center;
                    WebMap.VerticalAlignment = VerticalAlignment.Center;
                }
                else
                {
                    WebMap.Width = double.NaN;
                    WebMap.Height = double.NaN;
                    WebMap.ZoomFactor = 1.0;
                    WebMap.HorizontalAlignment = HorizontalAlignment.Stretch;
                    WebMap.VerticalAlignment = VerticalAlignment.Stretch;
                }
            };
        }
    }

    private async void StyleBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton btn && DataContext is MapViewModel vm)
        {
            var style = btn.Tag?.ToString() ?? "dark";
            vm.CurrentStyleKey = style;
            await vm.SetMapStyleAsync(style);
        }
    }

    private static string GetInlineHtml()
    {
        // Read from embedded resource
        var asm = typeof(MapView).Assembly;
        var name = asm.GetManifestResourceNames()
                      .FirstOrDefault(n => n.EndsWith("index.html"));
        if (name is null) return "<html><body>Map unavailable</body></html>";
        using var stream = asm.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
