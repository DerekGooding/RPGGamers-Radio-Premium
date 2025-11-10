using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.Services;
[Singleton]
internal class SnackbarService : ISnackbarService
{
    public TimeSpan DefaultTimeOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public SnackbarPresenter? GetSnackbarPresenter() => throw new NotImplementedException();
    public void SetSnackbarPresenter(SnackbarPresenter contentPresenter) => throw new NotImplementedException();
    public void Show(string title, string message, ControlAppearance appearance, IconElement? icon, TimeSpan timeout) => throw new NotImplementedException();
}
