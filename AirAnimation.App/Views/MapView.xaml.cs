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

        // Navigate to the bundled HTML
        var htmlPath = Path.Combine(AppContext.BaseDirectory, "Resources", "MapHtml", "index.html");
        if (File.Exists(htmlPath))
            WebMap.Source = new Uri(htmlPath);
        else
        {
            // Fallback: load from embedded resource
            var uri = new Uri("https://raw.githubusercontent.com/maplibre/maplibre-gl-js/main/README.md"); // placeholder
            // Inline the HTML directly
            var html = GetInlineHtml();
            WebMap.NavigateToString(html);
        }

        // Attach bridge
        if (DataContext is MapViewModel vm)
            vm.AttachWebView(WebMap);
    }

    private async void StyleBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && DataContext is MapViewModel vm)
            await vm.SetMapStyleAsync(btn.Tag?.ToString() ?? "dark");
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
