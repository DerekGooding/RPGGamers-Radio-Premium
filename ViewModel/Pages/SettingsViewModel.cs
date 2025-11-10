using GamerRadio.Services;
using Microsoft.Win32;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace GamerRadio.ViewModel.Pages;

[ViewModel, Singleton]
public partial class SettingsViewModel(NotificationService notificationService) : INavigationAware
{
    private readonly NotificationService _notificationService = notificationService;
    private bool _isInitialized;

    internal const string LibraryNamespace = "wpf.ui;";
    internal const string ThemesDictionaryPath = "pack://application:,,,/Wpf.Ui;component/Resources/Theme/";

    public Action<bool>? HandleMinimizeChange;

    [Bind(OnChangeMethodName = nameof(OnMinToTrayChanged))] private bool _minToTray;
    public void OnMinToTrayChanged(bool value) => HandleMinimizeChange?.Invoke(value);

    [Bind(OnChangeMethodName = nameof(OnNotificationCornerChanged))] private int _notificationCorner;
    public void OnNotificationCornerChanged(int value) => _notificationService.NotificationCorner = value;

    [Bind(OnChangeMethodName = nameof(OnIsNotificationEnabledChanged))] private bool _isNotificationEnabled = true;
    public void OnIsNotificationEnabledChanged(bool value) => _notificationService.IsEnabled = value;

    [Bind]private string _appVersion = string.Empty;
    [Bind] private ApplicationTheme _currentTheme = ApplicationTheme.Dark;

    public List<string> LocationOptions = [ "Top Right", "Bottom Right", "Bottom Left", "Top Left"];

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    private void InitializeViewModel()
    {
        CurrentTheme = ApplicationThemeManager.GetAppTheme();
        AppVersion = $"RPGGamer Radio Desktop - {GetVersion()}";

        _isInitialized = true;
    }

    [Command]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentTheme == ApplicationTheme.Light)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                CurrentTheme = ApplicationTheme.Light;
                HandleThemeChange?.Invoke(false);

                break;

            default:
                if (CurrentTheme == ApplicationTheme.Dark)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                CurrentTheme = ApplicationTheme.Dark;
                HandleThemeChange?.Invoke(true);

                break;
        }
    }

    public Action<bool>? HandleThemeChange;

    public string GetVersion()
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\");
        if (registryKey == null) return "1.0.0.0";

        foreach (var keyName in registryKey.GetSubKeyNames())
        {
            var programKey = registryKey.OpenSubKey(keyName);

            var displayName = programKey?.GetValue("DisplayName");
            var version = programKey?.GetValue("DisplayVersion");

            if (displayName != null && $"{displayName}" == "RPG Radio Premium")
                return $"{version}";

            programKey?.Close();
        }

        registryKey.Close();
        return "1.0.0.0";
    }
}
