using GamerRadio.Model;
using GamerRadio.Services;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages
{
    public partial class SongsViewModel(MediaElementService mediaElementService,
                                        SnackbarService snackbarService,
                                        FavoritesViewModel favoritesViewModel) : ObservableObject, INavigationAware
    {
        private readonly MediaElementService _mediaElementService = mediaElementService;
        private readonly SnackbarService _snackbarService = snackbarService;
        private readonly FavoritesViewModel _favoritesViewModel = favoritesViewModel;

        [ObservableProperty]
        private string _search = string.Empty;
        partial void OnSearchChanged(string value) => Query();

        [ObservableProperty]
        private List<SongImage> _songImages = [];

        [ObservableProperty]
        private List<GroupedSongImage> _groupedSongImages = [];

        [ObservableProperty]
        private bool _isSorted;
        partial void OnIsSortedChanged(bool value) => Query();

        public void OnNavigatedTo() => Query();
        public void OnNavigatedFrom() { }

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
            if(s.IsFavorite)
            {
                _favoritesViewModel.Add(s);
            }
            else
            {
                _favoritesViewModel.Remove(s);
            }
        }

        [RelayCommand]
        public void Ignore(SongImage? songImage)
        {
            if (songImage is not SongImage s) return;
            s.IsIgnored = !s.IsIgnored;
            string message = s.IsIgnored ? "is disabled!" : "Enabled again";
            _snackbarService.Show(s.Song.Title, message, ControlAppearance.Caution, null, TimeSpan.FromSeconds(1.5));
        }

        [RelayCommand]
        public void PlayRandomSong() => _mediaElementService.PlayRandomSong();

        [RelayCommand]
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
                => songImages.GroupBy(si => si.Song.Game)
                .OrderBy(g => g.Key)
                .Select(g => new GroupedSongImage
                {
                    Game = g.Key,
                    SongImages = new List<SongImage>(g.OrderBy(si => si.Song.Title))
                }).ToList();
    }
}
