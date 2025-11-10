using Wpf.Ui.Abstractions;

namespace GamerRadio.Services;

/// <summary>
/// Service that provides pages for navigation.
/// </summary>
[Singleton]
public class NavigationViewPageProvider : INavigationViewPageProvider
{
    /// <inheritdoc />
    public T? GetPage<T>() where T : class
        => !typeof(FrameworkElement).IsAssignableFrom(typeof(T))
            ? throw new InvalidOperationException("The page should be a WPF control.")
            : ((App)Application.Current).Get<T>();

    public object? GetPage(Type pageType) => ((App)Application.Current).Get(pageType);
}
