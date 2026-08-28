using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AirAnimation.App.Models;
using AirAnimation.App.Services;
using Microsoft.Win32;

namespace AirAnimation.App.Views;

public partial class AddCustomModelDialog : Window
{
    private readonly CustomModelService _customModelService;
    private string? _selectedFilePath;
    private double _rotX = 0;
    private double _rotY = 0;
    private double _rotZ = 0;

    public CustomTransportModel? CreatedModel { get; private set; }

    public AddCustomModelDialog(CustomModelService customModelService)
    {
        InitializeComponent();
        _customModelService = customModelService;
        UpdateRotationDisplay();
        ValidateAll();
    }

    private void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "3D Models (*.glb;*.gltf)|*.glb;*.gltf|Binary glTF (*.glb)|*.glb|glTF (*.gltf)|*.gltf|All Files (*.*)|*.*",
            Title = "Выберите 3D модель (Blender GLB/glTF)"
        };

        if (dlg.ShowDialog(this) == true)
        {
            _selectedFilePath = dlg.FileName;
            var fi = new FileInfo(_selectedFilePath);
            TxtSelectedFile.Text = fi.Name;
            TxtSelectedFile.Foreground = (Brush)FindResource("TextPrimaryBrush");
            TxtFileSize.Text = $"{fi.Length / 1024.0 / 1024.0:F2} MB";

            // If model name is empty, suggest filename without extension
            if (string.IsNullOrWhiteSpace(TxtModelName.Text))
            {
                var cleanName = Path.GetFileNameWithoutExtension(fi.Name)
                    .Replace('_', ' ')
                    .Replace('-', ' ');
                TxtModelName.Text = cleanName;
            }

            ValidateAll();
        }
    }

    private void ModelName_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateAll();
    }

    private void Speed_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateAll();
    }

    private void OrientationPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbOrientationPreset is null) return;

        switch (CmbOrientationPreset.SelectedIndex)
        {
            case 0: // Default Y-Up
                _rotX = 0; _rotY = 0; _rotZ = 0;
                break;
            case 1: // Flip 180 Y
                _rotX = 0; _rotY = 180; _rotZ = 0;
                break;
            case 2: // Blender Z-Up (+90 X)
                _rotX = 90; _rotY = 0; _rotZ = 0;
                break;
            case 3: // Blender Z-Down (-90 X)
                _rotX = -90; _rotY = 0; _rotZ = 0;
                break;
            case 4: // Upside down flip (180 X)
                _rotX = 180; _rotY = 0; _rotZ = 0;
                break;
            case 5: // +90 Y
                _rotX = 0; _rotY = 90; _rotZ = 0;
                break;
            case 6: // -90 Y
                _rotX = 0; _rotY = -90; _rotZ = 0;
                break;
        }

        UpdateRotationDisplay();
    }

    private void RotateX_Click(object sender, RoutedEventArgs e)
    {
        _rotX = (_rotX + 90) % 360;
        UpdateRotationDisplay();
    }

    private void RotateY_Click(object sender, RoutedEventArgs e)
    {
        _rotY = (_rotY + 90) % 360;
        UpdateRotationDisplay();
    }

    private void Rotate180_Click(object sender, RoutedEventArgs e)
    {
        _rotY = (_rotY + 180) % 360;
        UpdateRotationDisplay();
    }

    private void ResetRotation_Click(object sender, RoutedEventArgs e)
    {
        _rotX = 0;
        _rotY = 0;
        _rotZ = 0;
        if (CmbOrientationPreset is not null) CmbOrientationPreset.SelectedIndex = 0;
        UpdateRotationDisplay();
    }

    private void UpdateRotationDisplay()
    {
        if (TxtCurrentRotation is null) return;
        TxtCurrentRotation.Text = $"X: {_rotX:F0}° | Y: {_rotY:F0}°";
    }

    private void VehicleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ChkFollowRoads is null || TxtSpeed is null || CmbVehicleType is null) return;

        if (CmbVehicleType.SelectedItem is ComboBoxItem item)
        {
            var content = item.Content?.ToString() ?? "";
            if (content.Contains("Автомобиль"))
            {
                ChkFollowRoads.IsChecked = true;
                TxtSpeed.Text = "120";
            }
            else if (content.Contains("Авиация"))
            {
                ChkFollowRoads.IsChecked = false;
                TxtSpeed.Text = "850";
            }
            else if (content.Contains("Вертолёт"))
            {
                ChkFollowRoads.IsChecked = false;
                TxtSpeed.Text = "240";
            }
            else if (content.Contains("Водный"))
            {
                ChkFollowRoads.IsChecked = false;
                TxtSpeed.Text = "45";
            }
            else if (content.Contains("Космический"))
            {
                ChkFollowRoads.IsChecked = false;
                TxtSpeed.Text = "5000";
            }
            else if (content.Contains("Железнодорожный"))
            {
                ChkFollowRoads.IsChecked = false;
                TxtSpeed.Text = "160";
            }
            else
            {
                ChkFollowRoads.IsChecked = false;
                TxtSpeed.Text = "100";
            }
        }
    }

    private bool ValidateAll()
    {
        if (TxtModelName is null || TxtSpeed is null || BtnSave is null || ValidationPanel is null)
            return false;

        var isFileValid = false;
        var isNameValid = false;

        // 1. File Validation
        if (!string.IsNullOrWhiteSpace(_selectedFilePath))
        {
            var fileRes = _customModelService.ValidateFile(_selectedFilePath);
            isFileValid = fileRes.IsValid;
            if (!fileRes.IsValid)
            {
                ShowValidationError(fileRes.ErrorMessage!);
                BtnSave.IsEnabled = false;
                return false;
            }
        }

        // 2. Name Validation
        var name = TxtModelName.Text;
        var nameRes = _customModelService.ValidateName(name);
        isNameValid = nameRes.IsValid;

        if (!nameRes.IsValid)
        {
            ShowValidationError(nameRes.ErrorMessage!);
        }
        else
        {
            ShowValidationSuccess("Название свободно и готово к использованию");
        }

        var isSpeedValid = double.TryParse(TxtSpeed.Text, out var speed) && speed > 0;

        var canSave = isFileValid && isNameValid && isSpeedValid;
        BtnSave.IsEnabled = canSave;
        return canSave;
    }

    private void ShowValidationError(string message)
    {
        if (ValidationPanel is null || TxtValidationIcon is null || TxtValidationMessage is null) return;
        ValidationPanel.Visibility = Visibility.Visible;
        TxtValidationIcon.Text = "❌";
        TxtValidationMessage.Text = message;
        TxtValidationMessage.Foreground = (Brush)FindResource("DangerBrush");
    }

    private void ShowValidationSuccess(string message)
    {
        if (ValidationPanel is null || TxtValidationIcon is null || TxtValidationMessage is null) return;
        ValidationPanel.Visibility = Visibility.Visible;
        TxtValidationIcon.Text = "✔️";
        TxtValidationMessage.Text = message;
        TxtValidationMessage.Foreground = (Brush)FindResource("SuccessBrush");
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateAll() || string.IsNullOrWhiteSpace(_selectedFilePath)) return;

        BtnSave.IsEnabled = false;
        try
        {
            var (type, emoji) = GetVehicleTypeAndEmoji();
            var speed = double.TryParse(TxtSpeed.Text, out var s) ? s : 100;
            var followRoads = ChkFollowRoads.IsChecked == true;

            CreatedModel = await _customModelService.AddModelAsync(
                _selectedFilePath,
                TxtModelName.Text.Trim(),
                type,
                emoji,
                speed,
                followRoads,
                _rotX,
                _rotY,
                _rotZ);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            ShowValidationError($"Ошибка сохранения: {ex.Message}");
            BtnSave.IsEnabled = true;
        }
    }

    private (string type, string emoji) GetVehicleTypeAndEmoji()
    {
        var idx = CmbVehicleType.SelectedIndex;
        return idx switch
        {
            0 => ("Авто", "🚗"),
            1 => ("Авиация", "✈️"),
            2 => ("Авиация", "🚁"),
            3 => ("Море", "🚢"),
            4 => ("Космос", "🚀"),
            5 => ("Ж/д", "🚆"),
            _ => ("Экзотика", "🛸")
        };
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
