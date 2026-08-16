using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AirAnimation.App.Views;

public partial class TransportPickerView : UserControl
{
    public TransportPickerView() => InitializeComponent();

    private void TabsListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            // Find the ScrollViewer inside the ListBox
            if (VisualTreeHelper.GetChildrenCount(listBox) > 0)
            {
                var border = VisualTreeHelper.GetChild(listBox, 0) as Decorator;
                if (border?.Child is ScrollViewer scrollViewer)
                {
                    // Convert vertical scroll to horizontal scroll
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
                    e.Handled = true;
                }
            }
        }
    }
}
