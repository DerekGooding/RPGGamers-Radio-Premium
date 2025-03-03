using GamerRadio.ViewModel.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace GamerRadio.View.Pages;

public partial class FavoritesPage : INavigableView<FavoritesViewModel>
{
    public FavoritesViewModel ViewModel { get; }

    public FavoritesPage(FavoritesViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
