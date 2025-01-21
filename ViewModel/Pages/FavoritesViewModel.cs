using GamerRadio.Model;
using GamerRadio.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages;

public partial class FavoritesViewModel(MediaElementService mediaElementService,
                                        SnackbarService snackbarService) : ObservableObject
{
    private readonly MediaElementService _mediaElementService = mediaElementService;
    private readonly SnackbarService _snackbarService = snackbarService;

    [ObservableProperty]
    private ObservableCollection<SongImage> _favorites = [];

    public void Add(SongImage image) => Favorites.Add(image);
    public void Remove(SongImage image) => Favorites.Remove(image);

    [RelayCommand]
    public void PlayByButton(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        _mediaElementService.PlayMedia(s);
    }

    [RelayCommand]
    public void Favorite(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        s.IsFavorite = !s.IsFavorite;
        string message = s.IsFavorite ? "Saved to Favorites!" : "Removed from Favorites";
        _snackbarService.Show(s.Song.Title, message, ControlAppearance.Success, null, TimeSpan.FromSeconds(1.5));
        if (s.IsFavorite)
        {
            Add(s);
        }
        else
        {
            Remove(s);
        }
    }

    [RelayCommand]
    public void Ignore(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        s.IsIgnored = !s.IsIgnored;
        string message = s.IsIgnored ? "Disabled! But why are you disabling your favorites?" : "Enabled again";
        _snackbarService.Show(s.Song.Title, message, ControlAppearance.Caution, null, TimeSpan.FromSeconds(1.5));
    }
}
