using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.Services;
[Singleton]
internal class SnackbarService : ISnackbarService
{
    private SnackbarPresenter? _presenter;

    private Snackbar? _snackbar;

    /// <inheritdoc />
    public TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(5);

    /// <inheritdoc />
    public void SetSnackbarPresenter(SnackbarPresenter contentPresenter) => _presenter = contentPresenter;

    /// <inheritdoc />
    public SnackbarPresenter? GetSnackbarPresenter() => _presenter;

    /// <inheritdoc />
    public void Show(
        string title,
        string message,
        ControlAppearance appearance,
        IconElement? icon,
        TimeSpan timeout
    )
    {
        if (_presenter is null)
        {
            throw new InvalidOperationException("The SnackbarPresenter was never set");
        }

        _snackbar ??= new Snackbar(_presenter);

        _snackbar.SetCurrentValue(Snackbar.TitleProperty, title);
        _snackbar.SetCurrentValue(System.Windows.Controls.ContentControl.ContentProperty, message);
        _snackbar.SetCurrentValue(Snackbar.AppearanceProperty, appearance);
        _snackbar.SetCurrentValue(Snackbar.IconProperty, icon);
        _snackbar.SetCurrentValue(
            Snackbar.TimeoutProperty,
            timeout.TotalSeconds == 0 ? DefaultTimeOut : timeout
        );

        _snackbar.Show(true);
    }
}
