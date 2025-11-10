using GamerRadio.Services;
using GamerRadio.ViewModel.Pages;
using GamerRadio.ViewModel.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;

namespace GamerRadio.View.Windows;

public partial class MainWindow : INavigationWindow
{
    private readonly NotifyIcon _notifyIcon;
    private readonly MediaElementService _mediaElementService;
    private readonly SettingsViewModel _settingsViewModel;

    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationViewPageProvider pageService,
        INavigationService navigationService,
        MediaElementService mediaElementService,
        SnackbarService snackbarService,
        SettingsViewModel settingsViewModel
    )
    {
        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        var bitmap = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/radio.png"));
        _notifyIcon = new NotifyIcon
        {
            Icon = bitmap,
            ToolTip = "Game Radio Premium",
            ContextMenu = new ()
        };


        SetPageService(pageService);
        _mediaElementService = mediaElementService;
        //_mediaElementService.MediaElement = MyPlayer;
        navigationService.SetNavigationControl(RootNavigation);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        _settingsViewModel = settingsViewModel;
        _settingsViewModel.HandleMinimizeChange += MinimizeChange;
        SetupNotifyIcon();
        _notifyIcon.IsEnabled = _settingsViewModel.MinToTray;
    }

    #region INavigationWindow methods

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider pageService) => RootNavigation.SetPageProviderService(pageService);

    private void ShowWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    public void CloseWindow() => Close();

    #endregion INavigationWindow methods

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        _notifyIcon.Dispose();
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    INavigationView INavigationWindow.GetNavigation() => throw new NotImplementedException();

    public void SetServiceProvider(IServiceProvider serviceProvider) => throw new NotImplementedException();

    private void SetupNotifyIcon()
    {
        _notifyIcon.BeginInit();
        _notifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Show", Command = new RelayCommand(ShowWindow) });
        _notifyIcon.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
        _notifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Next Song", Command = new RelayCommand(_mediaElementService.PlayRandomSong) });
        _notifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Puase/Play", Command = new RelayCommand(_mediaElementService.Pause) });
        _notifyIcon.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
        _notifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Exit", Command = new RelayCommand(ExitApplication) });

        _notifyIcon.LeftDoubleClick += HandleLeftDoubleClick;
        _notifyIcon.EndInit();
    }

    private void HandleLeftDoubleClick(object? sender, RoutedEventArgs e) => ShowWindow();

    private void MinimizeChange(bool value)
    {
        _notifyIcon.IsEnabled = value;
    }

    private void ExitApplication()
    {
        _notifyIcon.Dispose();
        Application.Current.Shutdown();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        if (_settingsViewModel.MinToTray && WindowState == WindowState.Minimized)
        {
            Hide();
        }
    }

    void INavigationWindow.ShowWindow() => ShowWindow();
}
