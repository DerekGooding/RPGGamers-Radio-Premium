using System.Windows.Media.Animation;

namespace GamerRadio.View.Windows;

/// <summary>
/// Interaction logic for NotificationWindow.xaml
/// </summary>
public partial class NotificationWindow : Window
{
    private bool _isClosing;
    private int _count;

    private readonly DoubleAnimation _fadeOutAnimation = new()
    {
        From = 1,
        To = 0,
        Duration = TimeSpan.FromSeconds(0.5),
    };


    public NotificationWindow() => InitializeComponent();

    public async Task ShowNotificationAsync(string title, string message, int cornerIndex, int duration = 3000)
    {
        TitleTextBlock.Text = title;
        MessageTextBlock.Text = message;
        _isClosing = false;
        BeginAnimation(OpacityProperty, null);
        Opacity = 1;

        switch (cornerIndex)
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

        _count++;
        await Task.Delay(duration);
        if(_count == 1)
            ClosingAnimation();
        _count--;
    }

    protected void ClosingAnimation()
    {
        if (!_isClosing)
        {
            _isClosing = true;

            _fadeOutAnimation.Completed += (s, a) =>
            {
                if (_isClosing)
                {
                    Application.Current.Dispatcher.Invoke(() => Hide());
                }
            };

            BeginAnimation(OpacityProperty, _fadeOutAnimation);
        }
    }
}
