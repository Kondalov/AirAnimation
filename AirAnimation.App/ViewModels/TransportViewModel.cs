using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AirAnimation.App.Models;

namespace AirAnimation.App.ViewModels;

public sealed partial class TransportViewModel : ObservableObject
{
    public ObservableCollection<string> Categories { get; } = [];
    public ObservableCollection<TransportModel> FilteredTransports { get; } = [];

    [ObservableProperty] private TransportModel? selectedTransport;
    [ObservableProperty] private string selectedCategory = "Все";
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private double transportSize = 64; // Default 64px, range 28..160

    public event EventHandler<TransportModel>? TransportChanged;
    public event EventHandler<double>? TransportSizeChanged;

    public TransportViewModel()
    {
        var cats = new[] { "Все" }.Concat(
            TransportModel.All.Select(t => t.Category).Distinct()).ToList();
        foreach (var c in cats) Categories.Add(c);

        ApplyFilter();
    }

    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedTransportChanged(TransportModel? value)
    {
        if (value is not null)
            TransportChanged?.Invoke(this, value);
    }

    partial void OnTransportSizeChanged(double value) =>
        TransportSizeChanged?.Invoke(this, value);

    public void SelectTransport(string id)
    {
        SelectedCategory = "Все";
        ApplyFilter();
        SelectedTransport = FilteredTransports.FirstOrDefault(t => t.Id == id)
                         ?? TransportModel.All.FirstOrDefault(t => t.Id == id);
    }

    [RelayCommand]
    private void SelectItem(TransportModel? item)
    {
        if (item is not null) SelectedTransport = item;
    }

    private void ApplyFilter()
    {
        var prevSelected = SelectedTransport;
        FilteredTransports.Clear();
        var query = TransportModel.All.AsEnumerable();

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
