using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AirAnimation.App.ViewModels;

namespace AirAnimation.App.Views;

public partial class AnimationSettingsView : UserControl
{
    public AnimationSettingsView() => InitializeComponent();

    private void SpeedPreset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && DataContext is AnimationViewModel vm &&
            double.TryParse(btn.Tag?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var speed))
        {
            vm.SpeedMultiplier = speed;
        }
    }

    private void OrientationMode_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && DataContext is AnimationViewModel vm && btn.Tag is string mode)
        {
            vm.OrientationMode = mode;
        }
    }

    private void CameraMode_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && DataContext is AnimationViewModel vm && btn.Tag is string mode)
        {
            vm.CameraMode = mode;
        }
    }

    private void RouteDrawMode_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && DataContext is AnimationViewModel vm && btn.Tag is string mode)
        {
            vm.RouteDrawMode = mode;
        }
    }

    private void TrailStyle_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && DataContext is AnimationViewModel vm && btn.Tag is string style)
        {
            vm.TrailStyle = style;
        }
    }
}
