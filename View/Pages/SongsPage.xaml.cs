using GamerRadio.ViewModel.Pages;
using Wpf.Ui.Controls;

namespace GamerRadio.View.Pages;

public partial class SongsPage : INavigableView<SongsViewModel>
{
    public SongsViewModel ViewModel { get; }

    public SongsPage(SongsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void VirtualizingStackPanel_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
    {

    }
}
