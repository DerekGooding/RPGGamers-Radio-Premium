using GamerRadio.ViewModel.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace GamerRadio.View.Pages;

[Singleton]
public partial class SongsPage : INavigableView<SongsViewModel>
{
    public SongsViewModel ViewModel { get; }

    public SongsPage(SongsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
