using System.Windows.Media;

namespace GamerRadio.Model;

public partial class SongImage : ObservableObject
{
    public Song Song { get; set; } = new Song() { Game = "None", Title = "None" };
    public ImageSource? Source { get; set; }

    [ObservableProperty]
    private bool _isFavorite;
    [ObservableProperty]
    private bool _isIgnored;

    public SongImage(Song song, ImageSource source, bool isFavorite, bool isIgnored)
    {
        Song = song;
        Source = source;
        IsFavorite = isFavorite;
        IsIgnored = isIgnored;
    }

    public SongImage() { }
}
