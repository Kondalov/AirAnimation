using System.IO;
using System.Text.Json;
using AirAnimation.App.Models;

namespace AirAnimation.App.Services;

/// <summary>Saves/loads Route projects as JSON files (.airroute).</summary>
public static class ProjectService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly string _projectsDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AirAnimation");

    public static string ProjectsDirectory
    {
        get
        {
            Directory.CreateDirectory(_projectsDir);
            return _projectsDir;
        }
    }

    public static async Task SaveAsync(Models.Route route, string filePath, CancellationToken ct = default)
    {
        route.ModifiedAt = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(route, _options);
        await File.WriteAllTextAsync(filePath, json, ct);
    }

    public static async Task<Models.Route?> LoadAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath)) return null;
        var json = await File.ReadAllTextAsync(filePath, ct);
        return JsonSerializer.Deserialize<Models.Route>(json, _options);
    }

    public static IEnumerable<string> GetRecentProjects() =>
        Directory.Exists(_projectsDir)
            ? Directory.GetFiles(_projectsDir, "*.airroute")
                       .OrderByDescending(File.GetLastWriteTime)
                       .Take(20)
            : [];
}
