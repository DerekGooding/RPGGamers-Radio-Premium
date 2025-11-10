using GamerRadio.Services;
using GamerRadio.View.Pages;
using GamerRadio.View.Windows;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Windows;

[ViewModel, Singleton]
public partial class MainWindowViewModel
{
    [Bind] private string _applicationTitle = "RPGGamer Radio";

    [Bind] private ObservableCollection<object> _menuItems =
    [
        new NavigationViewItem()
        {
            Content = "Media",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(DashboardPage)
        },
        new NavigationViewItem()
        {
            Content = "Songs",
            Icon = new SymbolIcon { Symbol = SymbolRegular.MusicNote224 },
            TargetPageType = typeof(SongsPage)
        },
        new NavigationViewItem()
        {
            Content = "Favorites",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Star24 },
            TargetPageType = typeof(FavoritesPage)
        },
        new NavigationViewItem()
        {
            Content = "Twitch",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Chat24 },
            TargetPageType = typeof(TwitchPage)
        },
    ];

    [Bind]
    private ObservableCollection<object> _footerMenuItems =
    [
        new NavigationViewItem()
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        }
    ];

    [Bind]
    private ObservableCollection<MenuItem> _trayMenuItems =
    [
        new MenuItem { Header = "Home", Tag = "tray_home" }
    ];

    [Command(AcceptParameter = true)]
    public void ExitApplication(MainWindow mainWindow)
    {
        mainWindow.NotifyIcon.Dispose();
        Application.Current.Shutdown();
    }

    [Command(AcceptParameter = true)]
    public void ShowWindow(MainWindow mainWindow) => mainWindow.ShowWindow();

    [Command] public void TrayPause() => ((App)App.Current).Get<MediaElementService>()!.Pause();
    [Command] public void TrayPlayRandom() => ((App)App.Current).Get<MediaElementService>()!.PlayRandomSong();
}
