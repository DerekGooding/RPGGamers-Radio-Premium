using System.Windows;

namespace Radio_Leech
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}
