using System.Xml;
using AirAnimation.App.Models;

namespace AirAnimation.App.Services;

/// <summary>Parses GPX files into a list of Waypoints.</summary>
public static class GpxService
{
    public static List<Waypoint> Parse(string filePath)
    {
        var waypoints = new List<Waypoint>();
        var doc = new XmlDocument();
        doc.Load(filePath);

        var ns = new XmlNamespaceManager(doc.NameTable);
        // Support both GPX 1.0 and 1.1
        ns.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");
        ns.AddNamespace("gpx10", "http://www.topografix.com/GPX/1/0");

        // Try waypoints first
        var wpts = doc.SelectNodes("//gpx:wpt", ns) ?? doc.SelectNodes("//gpx10:wpt", ns);
        if (wpts?.Count > 0)
        {
            foreach (XmlNode wpt in wpts)
                waypoints.Add(ParseNode(wpt, waypoints.Count));
        }

        // Then track points
        var trkpts = doc.SelectNodes("//gpx:trkpt", ns) ?? doc.SelectNodes("//gpx10:trkpt", ns);
        if (trkpts?.Count > 0 && waypoints.Count == 0)
        {
            // Downsample track to max 200 points
            int step = Math.Max(1, trkpts.Count / 200);
            int order = 0;
            for (int i = 0; i < trkpts.Count; i += step)
                waypoints.Add(ParseNode(trkpts[i]!, order++));
        }

        return waypoints;
    }

    private static Waypoint ParseNode(XmlNode node, int order)
    {
        double lat = double.Parse(node.Attributes?["lat"]?.Value ?? "0", System.Globalization.CultureInfo.InvariantCulture);
        double lon = double.Parse(node.Attributes?["lon"]?.Value ?? "0", System.Globalization.CultureInfo.InvariantCulture);
        string? name = node.SelectSingleNode("name")?.InnerText?.Trim()
                    ?? node.SelectSingleNode("gpx:name", new XmlNamespaceManager(node.OwnerDocument!.NameTable))?.InnerText?.Trim();

        return new Waypoint
        {
            Latitude = lat,
            Longitude = lon,
            Name = string.IsNullOrEmpty(name) ? null : name,
            Order = order
        };
    }
}
