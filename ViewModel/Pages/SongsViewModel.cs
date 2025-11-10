using GamerRadio.Model;
using GamerRadio.Services;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages;

[ViewModel, Singleton]
public partial class SongsViewModel(MediaElementService mediaElementService,
                                    ISnackbarService snackbarService,
                                    FavoritesViewModel favoritesViewModel) : INavigationAware
{
    private readonly MediaElementService _mediaElementService = mediaElementService;
    private readonly ISnackbarService _snackbarService = snackbarService;
    private readonly FavoritesViewModel _favoritesViewModel = favoritesViewModel;

    [Bind(OnChangeMethodName = nameof(OnSearchChanged))] private string _search = string.Empty;
    public void OnSearchChanged() => Query();

    [Bind] private List<SongImage> _songImages = [];
    [Bind] private List<GroupedSongImage> _groupedSongImages = [];

    [Bind(OnChangeMethodName = nameof(OnIsSortedChanged))] private bool _isSorted;
    public void OnIsSortedChanged() => Query();

    public Task OnNavigatedToAsync()
    {
        Query();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    [Command(AcceptParameter = true)]
    public async void PlayByButton(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        await _mediaElementService.PlayMedia(s);
    }

    [Command(AcceptParameter = true)]
    public void Favorite(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        s.IsFavorite = !s.IsFavorite;
        var message = s.IsFavorite ? "Saved to Favorites!" : "Removed from Favorites";
        _snackbarService.Show(s.Song.Title, message, ControlAppearance.Success, null, TimeSpan.FromSeconds(1.5));
        if(s.IsFavorite)
        {
            _favoritesViewModel.Add(s);
        }
        else
        {
            _favoritesViewModel.Remove(s);
        }
    }

    [Command(AcceptParameter = true)]
    public void Ignore(SongImage? songImage)
    {
        if (songImage is not SongImage s) return;
        s.IsIgnored = !s.IsIgnored;
        var message = s.IsIgnored ? "Disabled!" : "Enabled again";
        _snackbarService.Show(s.Song.Title, message, ControlAppearance.Caution, null, TimeSpan.FromSeconds(1.5));
    }

    [Command]
    public void PlayRandomSong() => _mediaElementService.PlayRandomSong();

    [Command]
    public void Pause() => _mediaElementService.Pause();

    private void Query()
    {
        if (IsSorted)
        {
            GroupedSongImages = Grouped(SearchResults);
        }
        else
        {
            SongImages = SearchResults;
        }
    }

    private bool MatchesSearch(SongImage songImage)
        => songImage.Song.Game.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
        || songImage.Song.Title.Contains(Search, StringComparison.CurrentCultureIgnoreCase);

    private List<SongImage> SearchResults
        => string.IsNullOrEmpty(Search)
            ? _mediaElementService.SongImages
            : [.. _mediaElementService.SongImages.Where(MatchesSearch)];

    private List<GroupedSongImage> Grouped(List<SongImage> songImages)
            => [.. songImages.GroupBy(si => si.Song.Game)
            .OrderBy(g => g.Key)
            .Select(g => new GroupedSongImage
            {
                Game = g.Key,
                SongImages = [.. g.OrderBy(si => si.Song.Title)]
            })];
}
