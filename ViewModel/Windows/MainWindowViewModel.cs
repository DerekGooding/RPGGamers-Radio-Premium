using System.Collections.ObjectModel;
using GamerRadio.View.Pages;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _applicationTitle = "RPGGamer Radio";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems =
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

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems =
    [
        new NavigationViewItem()
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        }
    ];

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems =
    [
        new MenuItem { Header = "Home", Tag = "tray_home" }
    ];
}
