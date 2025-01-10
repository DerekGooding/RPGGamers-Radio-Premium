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

                    Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }

                public void Apply(
        ApplicationTheme applicationTheme,
        WindowBackdropType backgroundEffect = WindowBackdropType.Mica,
        bool updateAccent = true)
        {
            if (updateAccent)
            {
                ApplyAccent(
                    Color.FromRgb(0, 0, 200),
                    applicationTheme,
                    false
                );
            }

            if (applicationTheme == ApplicationTheme.Unknown)
            {
                return;
            }

            var themeDictionaryName = applicationTheme == ApplicationTheme.Dark ? "Dark" : "Light";


            bool isUpdated = UpdateDictionary("theme",
                new Uri(ThemesDictionaryPath + themeDictionaryName + ".xaml", UriKind.Absolute)
            );

            if (UiApplication.Current.MainWindow is Window mainWindow)
            {
                WindowBackgroundManager.UpdateBackground(mainWindow, applicationTheme, backgroundEffect);
            }
        }


        public void ApplyAccent(
        Color systemAccent,
        ApplicationTheme applicationTheme = ApplicationTheme.Light,
        bool systemGlassColor = false
        )
        {
            if (systemGlassColor)
            {
                // WindowGlassColor is little darker than accent color
                systemAccent = systemAccent.UpdateBrightness(6f);
            }

            Color primaryAccent;
            Color secondaryAccent;
            Color tertiaryAccent;

            if (applicationTheme == ApplicationTheme.Dark)
            {
                primaryAccent = systemAccent.Update(15f, -12f);
                secondaryAccent = systemAccent.Update(30f, -24f);
                tertiaryAccent = systemAccent.Update(45f, -36f);
            }
            else
            {
                primaryAccent = systemAccent.UpdateBrightness(-5f);
                secondaryAccent = systemAccent.UpdateBrightness(-10f);
                tertiaryAccent = systemAccent.UpdateBrightness(-15f);
            }

            UpdateColorResources(systemAccent, primaryAccent, secondaryAccent, tertiaryAccent);
        }

        private const double BackgroundBrightnessThresholdValue = 80d;

        private void UpdateColorResources(
                Color systemAccent,
                Color primaryAccent,
                Color secondaryAccent,
                Color tertiaryAccent
)
        {
            if (secondaryAccent.GetBrightness() > BackgroundBrightnessThresholdValue)
            {
                UiApplication.Current.Resources["TextOnAccentFillColorPrimary"] = Color.FromArgb(
                    0xFF,
                    0x00,
                    0x00,
                    0x00
                );
                UiApplication.Current.Resources["TextOnAccentFillColorSecondary"] = Color.FromArgb(
                    0x80,
                    0x00,
                    0x00,
                    0x00
                );
                UiApplication.Current.Resources["TextOnAccentFillColorDisabled"] = Color.FromArgb(
                    0x77,
                    0x00,
                    0x00,
                    0x00
                );
                UiApplication.Current.Resources["TextOnAccentFillColorSelectedText"] = Color.FromArgb(
                    0x00,
                    0x00,
                    0x00,
                    0x00
                );
                UiApplication.Current.Resources["AccentTextFillColorDisabled"] = Color.FromArgb(
                    0x5D,
                    0x00,
                    0x00,
                    0x00
                );
            }
            else
            {
                UiApplication.Current.Resources["TextOnAccentFillColorPrimary"] = Color.FromArgb(
                    0xFF,
                    0xFF,
                    0xFF,
                    0xFF
                );
                UiApplication.Current.Resources["TextOnAccentFillColorSecondary"] = Color.FromArgb(
                    0x80,
                    0xFF,
                    0xFF,
                    0xFF
                );
                UiApplication.Current.Resources["TextOnAccentFillColorDisabled"] = Color.FromArgb(
                    0x87,
                    0xFF,
                    0xFF,
                    0xFF
                );
                UiApplication.Current.Resources["TextOnAccentFillColorSelectedText"] = Color.FromArgb(
                    0xFF,
                    0xFF,
                    0xFF,
                    0xFF
                );
                UiApplication.Current.Resources["AccentTextFillColorDisabled"] = Color.FromArgb(
                    0x5D,
                    0xFF,
                    0xFF,
                    0xFF
                );
            }
            
            UiApplication.Current.Resources["SystemAccentColor"] = systemAccent;
            UiApplication.Current.Resources["SystemAccentColorPrimary"] = primaryAccent;
            UiApplication.Current.Resources["SystemAccentColorSecondary"] = secondaryAccent;
            UiApplication.Current.Resources["SystemAccentColorTertiary"] = tertiaryAccent;

            UiApplication.Current.Resources["SystemAccentBrush"]                          = new SolidColorBrush(secondaryAccent);
            UiApplication.Current.Resources["SystemFillColorAttentionBrush"]              = new SolidColorBrush(secondaryAccent);
            UiApplication.Current.Resources["AccentTextFillColorPrimaryBrush"]            = new SolidColorBrush(tertiaryAccent);
            UiApplication.Current.Resources["AccentTextFillColorSecondaryBrush"]          = new SolidColorBrush(tertiaryAccent);
            UiApplication.Current.Resources["AccentTextFillColorTertiaryBrush"]           = new SolidColorBrush(secondaryAccent);
            UiApplication.Current.Resources["AccentFillColorSelectedTextBackgroundBrush"] = new SolidColorBrush(systemAccent);
            UiApplication.Current.Resources["AccentFillColorDefaultBrush"]                = new SolidColorBrush(secondaryAccent);

            UiApplication.Current.Resources["AccentFillColorSecondaryBrush"]              = new SolidColorBrush(secondaryAccent) { Opacity = 0.9};
            UiApplication.Current.Resources["AccentFillColorTertiaryBrush"]               = new SolidColorBrush(secondaryAccent) { Opacity = 0.8 };
        }

        public bool UpdateDictionary(string resourceLookup, Uri? newResourceUri)
        {
            Collection<ResourceDictionary> applicationDictionaries = UiApplication
                .Current
                .Resources
                .MergedDictionaries;

            if (applicationDictionaries.Count == 0 || newResourceUri is null)
            {
                return false;
            }

            resourceLookup = resourceLookup.ToLower().Trim();

            for (int i = 0; i < applicationDictionaries.Count; i++)
            {
                string sourceUri;

                if (applicationDictionaries[i]?.Source != null)
                {
                    sourceUri = applicationDictionaries[i].Source.ToString().ToLower().Trim();

                    if (sourceUri.Contains(LibraryNamespace) && sourceUri.Contains(resourceLookup))
                    {
                        applicationDictionaries[i] = new() { Source = newResourceUri };

                        return true;
                    }
                }

                for (int j = 0; j < applicationDictionaries[i].MergedDictionaries.Count; j++)
                {
                    if (applicationDictionaries[i].MergedDictionaries[j]?.Source == null)
                    {
                        continue;
                    }

                    sourceUri = applicationDictionaries[i]
                        .MergedDictionaries[j]
                        .Source.ToString()
                        .ToLower()
                        .Trim();

                    if (!sourceUri.Contains(LibraryNamespace) || !sourceUri.Contains(resourceLookup))
                    {
                        continue;
                    }

                    applicationDictionaries[i].MergedDictionaries[j] = new() { Source = newResourceUri };

                    return true;
                }
            }

            return false;
        }
    }
}
