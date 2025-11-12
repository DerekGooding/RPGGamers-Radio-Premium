using GamerRadio.View.Windows;

namespace GamerRadio.Services;

[Singleton]
public class NotificationService
{
    readonly NotificationWindow notification = new()
    {
        Width = 300,
        Height = 100
    };


    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        set { _isEnabled = value; HandleChange?.Invoke(); }
    }

    public Action? HandleChange;
    public int NotificationCorner { get; set; }

    public async Task ShowNotificationAsync(string title, string message, int duration = 3000)
    {
        if(!IsEnabled) { return; }
        await Application.Current.Dispatcher.Invoke(async ()
            => await notification.ShowNotificationAsync(title, message, NotificationCorner, duration));
    }


}