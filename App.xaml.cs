using GamerRadio.Services;
using GamerRadio.View.Pages;
using GamerRadio.View.Windows;
using GamerRadio.ViewModel.Pages;
using GamerRadio.ViewModel.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace GamerRadio;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) ?? string.Empty))
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            // Page resolver service
            services.AddSingleton<INavigationViewPageProvider, NavigationViewPageProvider>();

            // Theme manipulation
            services.AddSingleton<IThemeService, ThemeService>();

            // TaskBar manipulation
            services.AddSingleton<ITaskBarService, TaskBarService>();

            services.AddSingleton<SnackbarService>();

            // Service containing navigation, same as INavigationWindow... but without window
            services.AddSingleton<INavigationService, NavigationService>();

            //services.AddSingleton<AudioService>();

            services.AddSingleton<DatabaseService>();
            services.AddSingleton<NotificationService>();
            services.AddSingleton<PreferencesService>();

            services.AddSingleton<MediaElementService>();

            // Main window with navigation
            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();

            services.AddSingleton<SongsPage>();
            services.AddSingleton<SongsViewModel>();

            services.AddSingleton<FavoritesPage>();
            services.AddSingleton<FavoritesViewModel>();

            services.AddSingleton<TwitchPage>();
            services.AddSingleton<TwitchViewModel>();
        }).Build();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T? GetService<T>()
        where T : class => _host.Services.GetService(typeof(T)) as T;

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        _host.Start();
        LoadPreferences();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        SavePreferences();
        await _host.StopAsync();

        _host.Dispose();
    }

    private void SavePreferences()
    {
        DashboardViewModel dashboardViewModel = _host.Services.GetService<DashboardViewModel>()!;
        SettingsViewModel SettingsViewModel = _host.Services.GetService<SettingsViewModel>()!;
        List<Model.SongImage> songs = _host.Services.GetService<MediaElementService>()!.SongImages;
        _host.Services.GetService<PreferencesService>()!.Save(SettingsViewModel.MinToTray, SettingsViewModel.IsNotificationEnabled, SettingsViewModel.NotificationCorner, dashboardViewModel.Volume,
            songs.Where(x=>x.IsFavorite).Select(x=>x.Song.Id), songs.Where(x => x.IsIgnored).Select(x => x.Song.Id));
    }

    private static void LoadPreferences()
    {
        DashboardViewModel dashboardViewModel = _host.Services.GetService<DashboardViewModel>()!;
        SettingsViewModel SettingsViewModel = _host.Services.GetService<SettingsViewModel>()!;
        FavoritesViewModel FavoritesViewModel = _host.Services.GetService<FavoritesViewModel>()!;
        MediaElementService songs = _host.Services.GetService<MediaElementService>()!;

        (bool MinToTray, bool NotificationOn, int NotificationCorner, double Volume, List<int> Favorites, List<int> Blocked)
            = _host.Services.GetService<PreferencesService>()!.Load();

        SettingsViewModel.MinToTray = MinToTray;
        SettingsViewModel.IsNotificationEnabled = NotificationOn;
        SettingsViewModel.NotificationCorner = NotificationCorner;
        dashboardViewModel.Volume = Volume;
        foreach (int id in Favorites)
        {
            songs.SongImages.First(x => x.Song.Id == id).IsFavorite = true;
            FavoritesViewModel.Add(songs.SongImages.First(x => x.Song.Id == id));
        }
        foreach (int id in Blocked)
        {
            songs.SongImages.First(x => x.Song.Id == id).IsIgnored = true;
        }
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}
