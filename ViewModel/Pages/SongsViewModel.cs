using GamerRadio.Model;
using GamerRadio.Services;
using Wpf.Ui.Controls;

namespace GamerRadio.ViewModel.Pages
{
    public partial class SongsViewModel(MediaElementService mediaElementService) : ObservableObject, INavigationAware
    {
        private readonly MediaElementService _mediaElementService = mediaElementService;

        [ObservableProperty]
        private string _search = string.Empty;
        partial void OnSearchChanged(string value)
            => SongImages = string.IsNullOrEmpty(value)
            ? _mediaElementService.SongImages
            : [.. _mediaElementService.SongImages.Where(MatchesSearch)];

        [ObservableProperty]
        private List<SongImage> _songImages = [];

        public void OnNavigatedTo() { SongImages = _mediaElementService.SongImages; }
        public void OnNavigatedFrom() { }

        [RelayCommand]
        public void PlayByButton(SongImage? songImage)
        {
            if (songImage is not SongImage s) return;
            _mediaElementService.PlayMedia(s);
        }

        [RelayCommand]
        public void PlayRandomSong() => _mediaElementService.PlayRandomSong();

        [RelayCommand]
        public void Pause() => _mediaElementService.Pause();

        private bool MatchesSearch(SongImage songImage)
            => songImage.Song.Game.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
            || songImage.Song.Title.Contains(Search, StringComparison.CurrentCultureIgnoreCase);
    }
}
