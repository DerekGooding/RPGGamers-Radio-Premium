using GamerRadio.ViewModel.Pages;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace GamerRadio.View.Pages;

public partial class DashboardPage : INavigableView<DashboardViewModel>
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e) => ViewModel.StartSeeking();

    private void Slider_PreviewMouseUp(object sender, MouseButtonEventArgs e) => ViewModel.StopSeeking(MySlider.Value);

    private void Slider_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Slider slider)
        {
            if (slider.Template.FindName("PART_Track", slider) is Track track)
            {
                track.PreviewMouseLeftButtonDown += Track_PreviewMouseLeftButtonDown;
            }
        }
    }

    private void Track_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Track track)
        {
            if (track.TemplatedParent is not Slider slider)
                return;

            var thumb = FindVisualChild<Thumb>(slider);
            if (thumb == null)
            {
                return;
            }

            double thumbPosition = thumb.TranslatePoint(new Point(0, 0), track).X;
            double thumbWidth = thumb.ActualWidth;
            Point mousePosition = e.GetPosition(track);

            if (mousePosition.X >= thumbPosition && mousePosition.X <= thumbPosition + thumbWidth)
            {
                return; 
            }

            double relativePosition = mousePosition.X / track.ActualWidth;
            slider.Value = (double)(slider.Minimum + (relativePosition * (slider.Maximum - slider.Minimum)));

            e.Handled = true;
        }
    }

    private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                return typedChild;
            }

            var result = FindVisualChild<T>(child);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
