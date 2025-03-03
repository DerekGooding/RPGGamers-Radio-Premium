using GamerRadio.Services;
using GamerRadio.ViewModel.Pages;
using GamerRadio.ViewModel.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GamerRadio.View.Windows;

public partial class MainWindow : INavigationWindow
{
    private readonly NotifyIcon _notifyIcon;
    private readonly MediaElementService _mediaElementService;
    private readonly SettingsViewModel _settingsViewModel;

    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        IPageService pageService,
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
        _notifyIcon = new NotifyIcon
        {
            Icon = new Icon("View/Icons/radio.ico"),
            Visible = true,
            Text = "My WPF App",
            ContextMenuStrip = new ContextMenuStrip()
        };
        SetupNotifyIcon();

        SetPageService(pageService);
        _mediaElementService = mediaElementService;
        _mediaElementService.MediaElement = MyPlayer;
        navigationService.SetNavigationControl(RootNavigation);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        _settingsViewModel = settingsViewModel;
    }

    #region INavigationWindow methods

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

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

    INavigationView INavigationWindow.GetNavigation()
    {
        throw new NotImplementedException();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    private void SetupNotifyIcon()
    {
        _notifyIcon.ContextMenuStrip!.Items.Add("Show", null, (s, e) => ShowWindow());
        _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => ExitApplication());
        _notifyIcon.ContextMenuStrip.Items.Add("Next Song", null, (s, e) => _mediaElementService.PlayRandomSong());

        _notifyIcon.DoubleClick += (s, e) => ShowWindow();
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
