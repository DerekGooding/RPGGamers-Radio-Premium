using GamerRadio.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages
{
    public partial class SettingsViewModel(NotificationService notificationService) : ObservableObject, INavigationAware
    {
        private NotificationService _notificationService = notificationService;
        private bool _isInitialized = false;

        [ObservableProperty]
        private bool _isNotificationEnabled = true;
        partial void OnIsNotificationEnabledChanged(bool value) => _notificationService.IsEnabled = value;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

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
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? string.Empty;
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
