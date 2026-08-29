using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AirAnimation.App.Models;
using AirAnimation.App.Services;
using AirAnimation.App.Views;

namespace AirAnimation.App.ViewModels;

public sealed partial class TransportViewModel : ObservableObject
{
    private readonly CustomModelService _customModelService;
    private readonly List<TransportModel> _customTransports = [];

    public ObservableCollection<string> Categories { get; } = [];
    public ObservableCollection<TransportModel> FilteredTransports { get; } = [];

    [ObservableProperty] private TransportModel? selectedTransport;
    [ObservableProperty] private string selectedCategory = "Все";
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private double transportSize = 64; // Default 64px, range 28..160

    public event EventHandler<TransportModel>? TransportChanged;
    public event EventHandler<double>? TransportSizeChanged;

    public TransportViewModel(CustomModelService? customModelService = null)
    {
        _customModelService = customModelService ?? new CustomModelService();

        RefreshCategories();
        _ = LoadCustomModelsAsync();
    }

    public async Task LoadCustomModelsAsync()
    {
        try
        {
            var customModels = await _customModelService.GetAllAsync();
            _customTransports.Clear();
            foreach (var cm in customModels)
            {
                _customTransports.Add(cm.ToTransportModel());
            }
        }
        catch
        {
            // Ignore load error on startup
        }

        RefreshCategories();
        ApplyFilter();
    }

    private void RefreshCategories()
    {
        var currentSelected = SelectedCategory;
        Categories.Clear();
        Categories.Add("Все");
        Categories.Add("Мои модели");

        var standardCats = TransportModel.All
            .Select(t => t.Category)
            .Distinct()
            .Where(c => c != "Мои модели");

        foreach (var c in standardCats)
        {
            Categories.Add(c);
        }

        if (Categories.Contains(currentSelected))
        {
            SelectedCategory = currentSelected;
        }
        else
        {
            SelectedCategory = "Все";
        }
    }

    public bool IsCustomSelected => SelectedTransport?.IsCustom == true;
    public string CurrentRotationText => SelectedTransport is not null 
        ? $"X: {SelectedTransport.RotationOffsetX:F0}° | Y: {SelectedTransport.RotationOffsetY:F0}° | Z: {SelectedTransport.RotationOffsetZ:F0}°" 
        : "X: 0° | Y: 0° | Z: 0°";

    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedTransportChanged(TransportModel? value)
    {
        OnPropertyChanged(nameof(IsCustomSelected));
        OnPropertyChanged(nameof(CurrentRotationText));
        if (value is not null)
            TransportChanged?.Invoke(this, value);
    }

    partial void OnTransportSizeChanged(double value) =>
        TransportSizeChanged?.Invoke(this, value);

    [RelayCommand]
    private async Task RotateXAsync()
    {
        if (SelectedTransport is null) return;
        SelectedTransport.RotationOffsetX = (SelectedTransport.RotationOffsetX + 90) % 360;
        await ApplyRotationChangeAsync();
    }

    [RelayCommand]
    private async Task RotateYAsync()
    {
        if (SelectedTransport is null) return;
        SelectedTransport.RotationOffsetY = (SelectedTransport.RotationOffsetY + 90) % 360;
        await ApplyRotationChangeAsync();
    }

    [RelayCommand]
    private async Task RotateZAsync()
    {
        if (SelectedTransport is null) return;
        SelectedTransport.RotationOffsetZ = (SelectedTransport.RotationOffsetZ + 90) % 360;
        await ApplyRotationChangeAsync();
    }

    [RelayCommand]
    private async Task Rotate180Async()
    {
        if (SelectedTransport is null) return;
        SelectedTransport.RotationOffsetY = (SelectedTransport.RotationOffsetY + 180) % 360;
        await ApplyRotationChangeAsync();
    }

    [RelayCommand]
    private async Task FlipUpsideDownAsync()
    {
        if (SelectedTransport is null) return;
        SelectedTransport.RotationOffsetX = (SelectedTransport.RotationOffsetX + 180) % 360;
        await ApplyRotationChangeAsync();
    }

    [RelayCommand]
    private async Task ResetRotationAsync()
    {
        if (SelectedTransport is null) return;
        SelectedTransport.RotationOffsetX = 0;
        SelectedTransport.RotationOffsetY = 0;
        SelectedTransport.RotationOffsetZ = 0;
        await ApplyRotationChangeAsync();
    }

    private async Task ApplyRotationChangeAsync()
    {
        if (SelectedTransport is null) return;
        OnPropertyChanged(nameof(CurrentRotationText));
        if (SelectedTransport.IsCustom)
        {
            await _customModelService.UpdateModelRotationAsync(
                SelectedTransport.Id, 
                SelectedTransport.RotationOffsetX, 
                SelectedTransport.RotationOffsetY, 
                SelectedTransport.RotationOffsetZ);
        }
        TransportChanged?.Invoke(this, SelectedTransport);
    }

    public void SelectTransport(string id)
    {
        SelectedCategory = "Все";
        ApplyFilter();
        SelectedTransport = FilteredTransports.FirstOrDefault(t => t.Id == id)
                         ?? _customTransports.FirstOrDefault(t => t.Id == id)
                         ?? TransportModel.All.FirstOrDefault(t => t.Id == id);
    }

    [RelayCommand]
    private void SelectItem(TransportModel? item)
    {
        if (item is not null) SelectedTransport = item;
    }

    [RelayCommand]
    private async Task AddCustomModelAsync()
    {
        var owner = Application.Current?.MainWindow;
        var dlg = new AddCustomModelDialog(_customModelService)
        {
            Owner = owner
        };

        if (dlg.ShowDialog() == true && dlg.CreatedModel is not null)
        {
            var transport = dlg.CreatedModel.ToTransportModel();
            _customTransports.Add(transport);
            RefreshCategories();
            SelectedCategory = "Мои модели";
            ApplyFilter();
            SelectedTransport = transport;
        }
    }

    [RelayCommand]
    private async Task DeleteCustomModelAsync(TransportModel? model)
    {
        if (model is null || !model.IsCustom) return;

        var result = MessageBox.Show(
            $"Вы действительно хотите удалить 3D модель «{model.Name}»?",
            "Удаление 3D модели",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        await _customModelService.DeleteModelAsync(model.Id);
        _customTransports.RemoveAll(t => t.Id == model.Id);

        if (SelectedTransport?.Id == model.Id)
        {
            SelectedTransport = _customTransports.LastOrDefault()
                             ?? TransportModel.All.FirstOrDefault();
        }

        RefreshCategories();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var prevSelected = SelectedTransport;
        FilteredTransports.Clear();

        var query = TransportModel.All.Concat(_customTransports).AsEnumerable();

        if (SelectedCategory != "Все")
            query = query.Where(t => t.Category == SelectedCategory);

        if (!string.IsNullOrWhiteSpace(SearchText))
            query = query.Where(t =>
                t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var t in query)
            FilteredTransports.Add(t);

        if (prevSelected is not null && FilteredTransports.Contains(prevSelected))
        {
            SelectedTransport = prevSelected;
        }
        else if (prevSelected is not null)
        {
            SelectedTransport = prevSelected;
        }
        else if (FilteredTransports.Count > 0)
        {
            SelectedTransport = FilteredTransports[0];
        }
    }
}
