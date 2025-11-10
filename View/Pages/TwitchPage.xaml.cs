using GamerRadio.ViewModel.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace GamerRadio.View.Pages;

[Singleton]
public partial class TwitchPage : INavigableView<TwitchViewModel>
{
    public TwitchViewModel ViewModel { get; }

    public TwitchPage(TwitchViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
