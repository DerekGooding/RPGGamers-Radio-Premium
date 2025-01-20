using System.Windows.Media.Animation;

namespace GamerRadio.View.Windows;

/// <summary>
/// Interaction logic for NotificationWindow.xaml
/// </summary>
public partial class NotificationWindow : Window
{
    public NotificationWindow(string title, string message)
    {
        InitializeComponent();
        TitleTextBlock.Text = title;
        MessageTextBlock.Text = message;
    }

    public async Task ShowNotificationAsync(int cornerIndex, int duration = 3000)
    {
        switch(cornerIndex)
        {
            case 0: //Top Right Corner
                Top = SystemParameters.WorkArea.Top + 10;
                Left = SystemParameters.WorkArea.Right - Width - 10;
                break;
            case 1: //Top Left Corner
                Top = SystemParameters.WorkArea.Top + 10;
                Left = SystemParameters.WorkArea.Left + 10;
                break;
            case 2: //Bottom Right Corner
                Top = SystemParameters.WorkArea.Bottom - Height - 10;
                Left = SystemParameters.WorkArea.Right - Width - 10;
                break;
            default: //Bottom Left Corner
                Top = SystemParameters.WorkArea.Bottom - Height - 10;
                Left = SystemParameters.WorkArea.Left + 10;
                break;
        }

        Show();

        await Task.Delay(duration);
        Close();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true; // Cancel the default closing action

        // Create fade-out animation
        var fadeOutAnimation = new DoubleAnimation
        {
            From = Opacity,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.5)
        };

        fadeOutAnimation.Completed += (s, a) =>
        {
            e.Cancel = false; // Allow the window to close after the animation
            Close();
        };

        BeginAnimation(OpacityProperty, fadeOutAnimation);
    }
}
