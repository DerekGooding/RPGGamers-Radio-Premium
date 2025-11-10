using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.Services;

[Singleton]
internal class NavigationService : INavigationService
{
    public INavigationView GetNavigationControl() => throw new NotImplementedException();
    public bool GoBack() => throw new NotImplementedException();
    public bool Navigate(Type pageType) => throw new NotImplementedException();
    public bool Navigate(Type pageType, object? dataContext) => throw new NotImplementedException();
    public bool Navigate(string pageIdOrTargetTag) => throw new NotImplementedException();
    public bool Navigate(string pageIdOrTargetTag, object? dataContext) => throw new NotImplementedException();
    public bool NavigateWithHierarchy(Type pageType) => throw new NotImplementedException();
    public bool NavigateWithHierarchy(Type pageType, object? dataContext) => throw new NotImplementedException();
    public void SetNavigationControl(INavigationView navigation) => throw new NotImplementedException();
}
