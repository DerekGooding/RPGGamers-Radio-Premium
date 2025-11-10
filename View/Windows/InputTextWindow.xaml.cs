using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Windows;

/// <summary>
/// Interaction logic for InputTextWindow.xaml
/// </summary>
public partial class InputTextWindow : FluentWindow
{
    public InputTextWindow(string initial)
    {
        InitializeComponent();
        APIField.Text = initial;
        Result = initial;
    }

    public string Result { get; private set; } = string.Empty;

    private void OnOkClicked(object sender, RoutedEventArgs e)
    {
        Result = APIField.Text;
        DialogResult = true;
    }

    private void OnCancelClicked(object sender, RoutedEventArgs e) => DialogResult = false;
}
