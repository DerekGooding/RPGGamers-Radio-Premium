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

[Singleton]
public partial class MainWindow : INavigationWindow
{
    public readonly NotifyIcon NotifyIcon;
    private readonly MediaElementService _mediaElementService;
    private readonly SettingsViewModel _settingsViewModel;

    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationViewPageProvider pageService,
        INavigationService navigationService,
        MediaElementService mediaElementService,
        ISnackbarService snackbarService,
        SettingsViewModel settingsViewModel
    )
    {
        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        var bitmap = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/radio.png"));
        NotifyIcon = new NotifyIcon
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
        NotifyIcon.IsEnabled = _settingsViewModel.MinToTray;

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_settingsViewModel.MinToTray)
        {
            NotifyIcon.Register();
        }
    }

    #region INavigationWindow methods

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider pageService) => RootNavigation.SetPageProviderService(pageService);

    public void ShowWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    public void CloseWindow() => Close();
    void INavigationWindow.ShowWindow() => ShowWindow();

    #endregion INavigationWindow methods

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        NotifyIcon.Dispose();
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider) => throw new NotImplementedException();

    private void SetupNotifyIcon()
    {
        NotifyIcon.BeginInit();
        NotifyIcon.ContextMenu.Items.Clear();
        NotifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Show", Command = new Command_ShowWindow(ViewModel), CommandParameter = this });
        NotifyIcon.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
        NotifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Next Song", Command = new Command_TrayPlayRandom(ViewModel) }); 
        NotifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Pause/Play", Command = new Command_TrayPause(ViewModel) });
        NotifyIcon.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
        NotifyIcon.ContextMenu.Items.Add(new MenuItem { Header = "Exit", Command = new Command_ExitApplication(ViewModel), CommandParameter = this });

        NotifyIcon.LeftDoubleClick += HandleLeftDoubleClick;
        NotifyIcon.RightClick += NotifyIcon_RightClick;
        NotifyIcon.EndInit();
    }

    private void NotifyIcon_RightClick([System.Diagnostics.CodeAnalysis.NotNull] NotifyIcon sender, RoutedEventArgs e) => NotifyIcon.ContextMenu.IsOpen = true;
    private void HandleLeftDoubleClick([System.Diagnostics.CodeAnalysis.NotNull] NotifyIcon sender, RoutedEventArgs e) => ShowWindow();

    private void MinimizeChange(bool value)
    {
        NotifyIcon.IsEnabled = value;

        if (value)
        {
            NotifyIcon.Register();
        }
        else
        {
            NotifyIcon.Unregister();
        }
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        if (_settingsViewModel.MinToTray && WindowState == WindowState.Minimized)
        {
            Hide();
        }
    }
}
