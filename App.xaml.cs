using GamerRadio.Services;
using GamerRadio.ViewModel.Pages;
using GamerRadio.Generated;
using GamerRadio.View.Windows;
using GamerRadio.View.Pages;

namespace GamerRadio;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private Host? _host;
    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public T? Get<T>() where T : class => _host?.Get<T>();

    public object? Get(Type type)
    {
        var method = typeof(Host)
            .GetMethod("Get", Type.EmptyTypes)
            ?.MakeGenericMethod(type);

        return method?.Invoke(_host, null);
    }

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        _host = Host.Initialize();
        LoadPreferences();

        var main = _host.Get<MainWindow>();
        main.Show();
        main.Navigate(typeof(DashboardPage));
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private void OnExit(object sender, ExitEventArgs e)
    {
        SavePreferences();
        Get<MediaElementService>()!.Dispose();
    }

    private void SavePreferences()
    {
        var dashboardViewModel = Get<DashboardViewModel>()!;
        var SettingsViewModel = Get<SettingsViewModel>()!;
        var songs = Get<MediaElementService>()!.SongImages;
        Get<PreferencesService>()!.Save(SettingsViewModel.MinToTray, SettingsViewModel.IsNotificationEnabled, SettingsViewModel.NotificationCorner, dashboardViewModel.Volume,
            songs.Where(x=>x.IsFavorite).Select(x=>x.Song.Id), songs.Where(x => x.IsIgnored).Select(x => x.Song.Id));
    }

    private void LoadPreferences()
    {
        var dashboardViewModel = Get<DashboardViewModel>();
        var SettingsViewModel = Get<SettingsViewModel>();
        var FavoritesViewModel = Get<FavoritesViewModel>();
        var songs = Get<MediaElementService>();

        (var MinToTray, var NotificationOn, var NotificationCorner, var Volume, var Favorites, var Blocked)
            = Get<PreferencesService>()!.Load();

        SettingsViewModel.MinToTray = MinToTray;
        SettingsViewModel.IsNotificationEnabled = NotificationOn;
        SettingsViewModel.NotificationCorner = NotificationCorner;
        dashboardViewModel.Volume = Volume;
        foreach (var id in Favorites)
        {
            songs.SongImages.First(x => x.Song.Id == id).IsFavorite = true;
            FavoritesViewModel.Add(songs.SongImages.First(x => x.Song.Id == id));
        }
        foreach (var id in Blocked)
        {
            songs.SongImages.First(x => x.Song.Id == id).IsIgnored = true;
        }
    }
}
