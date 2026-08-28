using System.IO;
using System.Text.Json;
using AirAnimation.App.Models;

namespace AirAnimation.App.Services;

/// <summary>
/// Service managing user-imported 3D models: persistence, file storage, and smart validation.
/// </summary>
public sealed class CustomModelService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly string _modelsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AirAnimation",
        "CustomModels");

    private static readonly string _registryPath = Path.Combine(_modelsDir, "custom_models.json");

    private readonly List<CustomTransportModel> _models = [];
    private readonly Lock _lock = new();

    public static string CustomModelsDirectory
    {
        get
        {
            Directory.CreateDirectory(_modelsDir);
            return _modelsDir;
        }
    }

    public async Task<IReadOnlyList<CustomTransportModel>> GetAllAsync()
    {
        lock (_lock)
        {
            if (_models.Count > 0)
                return [.. _models];
        }

        await LoadRegistryAsync();

        lock (_lock)
        {
            return [.. _models];
        }
    }

    public async Task<CustomTransportModel> AddModelAsync(
        string sourceFilePath,
        string name,
        string vehicleType,
        string emoji,
        double defaultSpeed,
        bool followRoads,
        double rotationOffsetX = 0,
        double rotationOffsetY = 0,
        double rotationOffsetZ = 0)
    {
        var fileValidation = ValidateFile(sourceFilePath);
        if (!fileValidation.IsValid)
            throw new ArgumentException(fileValidation.ErrorMessage, nameof(sourceFilePath));

        var nameValidation = ValidateName(name);
        if (!nameValidation.IsValid)
            throw new ArgumentException(nameValidation.ErrorMessage, nameof(name));

        var dir = CustomModelsDirectory;
        var ext = Path.GetExtension(sourceFilePath).ToLowerInvariant();
        var id = "custom_" + Guid.NewGuid().ToString("N");
        var targetFileName = $"{id}{ext}";
        var targetFilePath = Path.Combine(dir, targetFileName);

        // Copy source file to application storage
        await using (var sourceStream = File.OpenRead(sourceFilePath))
        await using (var targetStream = File.Create(targetFilePath))
        {
            await sourceStream.CopyToAsync(targetStream);
        }

        var fileInfo = new FileInfo(targetFilePath);
        var customModel = new CustomTransportModel
        {
            Id = id,
            Name = name.Trim(),
            FileName = targetFileName,
            VehicleType = vehicleType,
            Emoji = string.IsNullOrWhiteSpace(emoji) ? "🚗" : emoji,
            DefaultSpeed = defaultSpeed > 0 ? defaultSpeed : 100,
            FollowRoads = followRoads,
            RotationOffsetX = rotationOffsetX,
            RotationOffsetY = rotationOffsetY,
            RotationOffsetZ = rotationOffsetZ,
            FileSizeBytes = fileInfo.Length,
            CreatedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _models.Add(customModel);
        }

        await SaveRegistryAsync();
        return customModel;
    }

    public async Task<bool> DeleteModelAsync(string id)
    {
        CustomTransportModel? toRemove;
        lock (_lock)
        {
            toRemove = _models.FirstOrDefault(m => m.Id == id);
            if (toRemove is null) return false;
            _models.Remove(toRemove);
        }

        // Delete underlying file
        try
        {
            var filePath = Path.Combine(CustomModelsDirectory, toRemove.FileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // Ignore file delete failure if locked
        }

        await SaveRegistryAsync();
        return true;
    }

    public async Task<bool> UpdateModelRotationAsync(string id, double rotX, double rotY, double rotZ)
    {
        lock (_lock)
        {
            var model = _models.FirstOrDefault(m => m.Id == id);
            if (model is null) return false;
            model.RotationOffsetX = rotX;
            model.RotationOffsetY = rotY;
            model.RotationOffsetZ = rotZ;
        }

        await SaveRegistryAsync();
        return true;
    }

    public (bool IsValid, string? ErrorMessage) ValidateName(string? name, string? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, "Введите название модели.");

        var trimmed = name.Trim();

        if (trimmed.Length < 2)
            return (false, "Название должно содержать минимум 2 символа.");

        if (trimmed.Length > 50)
            return (false, "Название не должно превышать 50 символов.");

        var invalidChars = Path.GetInvalidFileNameChars();
        if (trimmed.IndexOfAny(invalidChars) >= 0)
            return (false, "Название содержит недопустимые символы (\\ / : * ? \" < > |).");

        // Check against built-in transport catalog
        var builtInMatch = TransportModel.All.FirstOrDefault(t =>
            string.Equals(t.Name.Trim(), trimmed, StringComparison.OrdinalIgnoreCase));
        if (builtInMatch is not null)
            return (false, $"Модель с названием «{builtInMatch.Name}» уже есть в стандартном каталоге.");

        // Check against custom models
        lock (_lock)
        {
            var customMatch = _models.FirstOrDefault(m =>
                m.Id != excludeId && string.Equals(m.Name.Trim(), trimmed, StringComparison.OrdinalIgnoreCase));
            if (customMatch is not null)
                return (false, $"Пользовательская модель с названием «{customMatch.Name}» уже существует.");
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidateFile(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return (false, "Файл не выбран.");

        if (!File.Exists(filePath))
            return (false, "Выбранный файл не найден.");

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        if (ext is not (".glb" or ".gltf"))
            return (false, "Поддерживаются только 3D форматы glTF/GLB (.glb, .gltf).");

        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
            return (false, "Файл пуст (0 байт).");

        if (fileInfo.Length > 150 * 1024 * 1024)
            return (false, "Размер файла превышает лимит 150 МБ.");

        if (ext == ".glb")
        {
            try
            {
                using var fs = File.OpenRead(filePath);
                Span<byte> magic = stackalloc byte[4];
                if (fs.Read(magic) == 4)
                {
                    // GLB magic is 0x46546C67 ("glTF")
                    if (magic[0] != 0x67 || magic[1] != 0x6C || magic[2] != 0x54 || magic[3] != 0x46)
                    {
                        return (false, "Файл не является корректным бинарным 3D файлом glTF (.glb).");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка чтения файла: {ex.Message}");
            }
        }

        return (true, null);
    }

    private async Task LoadRegistryAsync()
    {
        if (!File.Exists(_registryPath)) return;

        try
        {
            var json = await File.ReadAllTextAsync(_registryPath);
            var loaded = JsonSerializer.Deserialize<List<CustomTransportModel>>(json, _jsonOptions);
            if (loaded is not null)
            {
                lock (_lock)
                {
                    _models.Clear();
                    _models.AddRange(loaded);
                }
            }
        }
        catch
        {
            // If registry corrupted, start fresh
        }
    }

    private async Task SaveRegistryAsync()
    {
        try
        {
            Directory.CreateDirectory(_modelsDir);
            List<CustomTransportModel> snapshot;
            lock (_lock)
            {
                snapshot = [.. _models];
            }
            var json = JsonSerializer.Serialize(snapshot, _jsonOptions);
            await File.WriteAllTextAsync(_registryPath, json);
        }
        catch
        {
            // Ignore registry save error
        }
    }
}
