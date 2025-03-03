using GamerRadio.Services;
using Microsoft.Win32;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages
{
    public partial class SettingsViewModel(NotificationService notificationService) : ObservableObject, INavigationAware
    {
        private readonly NotificationService _notificationService = notificationService;
        private bool _isInitialized;

        internal const string LibraryNamespace = "wpf.ui;";
        internal const string ThemesDictionaryPath = "pack://application:,,,/Wpf.Ui;component/Resources/Theme/";

        public Action<bool>? HandleMinimizeChange;

        [ObservableProperty]
        private bool _minToTray;
        partial void OnMinToTrayChanged(bool value) => HandleMinimizeChange?.Invoke(value);

        [ObservableProperty]
        private int _notificationCorner;
        partial void OnNotificationCornerChanged(int value) => _notificationService.NotificationCorner = value;

        [ObservableProperty]
        private bool _isNotificationEnabled = true;
        partial void OnIsNotificationEnabledChanged(bool value) => _notificationService.IsEnabled = value;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Dark;

        public List<string> LocationOptions = [ "Top Right", "Bottom Right", "Bottom Left", "Top Left"];

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"RPGGamer Radio Desktop - {GetVersion()}";

            _isInitialized = true;
        }

        [RelayCommand]
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
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\");
            if (registryKey == null) return "1.0.0.0";

            foreach (string keyName in registryKey.GetSubKeyNames())
            {
                RegistryKey? programKey = registryKey.OpenSubKey(keyName);

                object? displayName = programKey?.GetValue("DisplayName");
                object? version = programKey?.GetValue("DisplayVersion");

                if (displayName != null && $"{displayName}" == "RPG Radio Premium")
                    return $"{version}";

                programKey?.Close();
            }

            registryKey.Close();
            return "1.0.0.0";
        }
    }
}
