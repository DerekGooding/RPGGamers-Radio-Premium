using GamerRadio.Model;
using GamerRadio.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages;

[ViewModel, Singleton]
public partial class FavoritesViewModel(MediaElementService mediaElementService, ISnackbarService snackbarService)
{
    private readonly MediaElementService _mediaElementService = mediaElementService;
    private readonly ISnackbarService _snackbarService = snackbarService;

    [Bind] private ObservableCollection<SongImage> _favorites = [];

    public void Add(SongImage image) => Favorites.Add(image);
    public void Remove(SongImage image) => Favorites.Remove(image);

    [Command(AcceptParameter = true)]
    public async void FavoritesPlayByButton(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        await _mediaElementService.PlayMedia(s);
    }

    [Command(AcceptParameter = true)]
    public void SelectFavorite(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        s.IsFavorite = !s.IsFavorite;
        var message = s.IsFavorite ? "Saved to Favorites!" : "Removed from Favorites";
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

    [Command(AcceptParameter = true)]
    public void FavoritesIgnore(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        s.IsIgnored = !s.IsIgnored;
        var message = s.IsIgnored ? "Disabled! But why are you disabling your favorites?" : "Enabled again";
        _snackbarService.Show(s.Song.Title, message, ControlAppearance.Caution, null, TimeSpan.FromSeconds(1.5));
    }
}
