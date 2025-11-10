using System.Windows.Media;

namespace GamerRadio.Model;

[ViewModel]
public partial class SongImage
{
    public Song Song { get; set; } = new Song() { Game = "None", Title = "None" };
    public ImageSource? Source { get; set; }

    [Bind] private bool _isFavorite;
    [Bind] private bool _isIgnored;

    public SongImage(Song song, ImageSource source, bool isFavorite, bool isIgnored)
    {
        Song = song;
        Source = source;
        IsFavorite = isFavorite;
        IsIgnored = isIgnored;
    }

    public SongImage() { }
}
