using GamerRadio.Services;
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
            AppVersion = $"RPGGamer Radio Desktop - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return "1.0.0.1";
            //return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
            //    ?? string.Empty;
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

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }
    }
}
