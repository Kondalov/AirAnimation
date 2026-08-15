using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AirAnimation.App.Views;

public partial class RouteEditorView : UserControl
{
    private static readonly Regex _lettersOnlyRegex = new(@"^[a-zA-Zа-яА-ЯёЁ\s\-]+$");

    public RouteEditorView()
    {
        InitializeComponent();
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Cancel input if it contains numbers or non-letter/space/hyphen characters
        if (!_lettersOnlyRegex.IsMatch(e.Text))
        {
            e.Handled = true;
        }
    }

    private void OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!_lettersOnlyRegex.IsMatch(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }
}
