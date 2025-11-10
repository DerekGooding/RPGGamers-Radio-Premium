using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.Services;

[Singleton]
internal class NavigationService : INavigationService
{
    protected INavigationView? NavigationControl { get; set; }

    public INavigationView GetNavigationControl() => NavigationControl ?? throw new ArgumentNullException("NavigationControl");

    public void SetNavigationControl(INavigationView navigation)
    {
        NavigationControl = navigation;
    }

    public bool Navigate(Type pageType)
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.Navigate(pageType);
    }

    public bool Navigate(Type pageType, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.Navigate(pageType, dataContext);
    }

    public bool Navigate(string pageTag)
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.Navigate(pageTag);
    }

    public bool Navigate(string pageTag, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.Navigate(pageTag, dataContext);
    }

    public bool GoBack()
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.GoBack();
    }

    public bool NavigateWithHierarchy(Type pageType)
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.NavigateWithHierarchy(pageType);
    }

    public bool NavigateWithHierarchy(Type pageType, object? dataContext)
    {
        ThrowIfNavigationControlIsNull();
        return NavigationControl.NavigateWithHierarchy(pageType, dataContext);
    }

    protected void ThrowIfNavigationControlIsNull()
    {
        if (NavigationControl == null)
        {
            throw new ArgumentNullException("NavigationControl");
        }
    }
}
