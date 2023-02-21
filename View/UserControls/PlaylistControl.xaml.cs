using Radio_Leech.Model.Database;
using System.Windows;
using System.Windows.Controls;

namespace Radio_Leech.View.UserControls
{
    public partial class PlaylistControl : UserControl
    {
        public Playlist Playlist {
            get { return (Playlist)GetValue(Property); }
            set { SetValue(Property, value); }
        }
        public static readonly DependencyProperty Property =
            DependencyProperty.Register("Playlist", typeof(Playlist),
                typeof(PlaylistControl), new PropertyMetadata(null, SetValues));
        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlaylistControl playlistControl)
                playlistControl.DataContext = playlistControl.Playlist;
        }
        public PlaylistControl() => InitializeComponent();
    }
}
