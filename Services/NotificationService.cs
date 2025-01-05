namespace GamerRadio.Services;

public class NotificationService
{
    public async Task ShowNotificationAsync(string title, string message, int duration = 3000)
        => await Application.Current.Dispatcher.Invoke(async () =>
            {
                var notification = new View.Windows.NotificationWindow(title, message)
                {
                    Width = 300,
                    Height = 100
                };
                await notification.ShowNotificationAsync(duration);
            });
}