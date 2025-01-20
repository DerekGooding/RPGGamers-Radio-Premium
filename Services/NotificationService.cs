namespace GamerRadio.Services;

public class NotificationService
{
    private bool isEnabled = true;
    public bool IsEnabled
    {
        get { return isEnabled; }
        set { isEnabled = value; HandleChange?.Invoke(); }
    }

    public async Task ShowNotificationAsync(string title, string message, int duration = 3000)
    {
        if(!IsEnabled) { return; }
        await Application.Current.Dispatcher.Invoke(async () =>
                {
                    var notification = new View.Windows.NotificationWindow(title, message)
                    {
                        Width = 300,
                        Height = 100
                    };
                    await notification.ShowNotificationAsync(NotificationCorner, duration);
                });
    }

    public Action? HandleChange;

    public int NotificationCorner { get; set; } = 0;
}