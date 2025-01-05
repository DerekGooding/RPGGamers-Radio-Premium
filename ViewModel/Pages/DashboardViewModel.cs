using GamerRadio.Model;
using GamerRadio.Services;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Threading;

namespace GamerRadio.ViewModel.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly MediaElementService _mediaElementService;

        private bool _isSeeking;

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private string _duration = "00:00";

        [ObservableProperty]
        private string _currentPoint = "00:00";

        [ObservableProperty]
        private double _progress;

        [ObservableProperty]
        private SongImage _currentlyPlaying = new() { Song = new() { Game = "None", Title = "None" } };

        [ObservableProperty]
        private ImageSource? _wavRender;

        [ObservableProperty]
        private double _volume = 1.0;
        partial void OnVolumeChanged(double value)
        {
            if (_mediaElementService.MediaElement == null) return;
            _mediaElementService.MediaElement.Volume = value;
        }

        public DashboardViewModel(MediaElementService mediaElementService)
        {
            _mediaElementService = mediaElementService;
            _mediaElementService.SongChange += HandleSongChange;
            _mediaElementService.PlayStatusChange += HandlePlayStatusChange;

            StartTimer();
        }

        private void HandleSongChange(object? sender, EventArgs e)
        {
            if (sender is not SongImage song) return;
            CurrentlyPlaying = song;
        }
        private void HandlePlayStatusChange(object? sender, EventArgs e)
        {
            if (sender is bool isPlaying)
            {
                IsPlaying = isPlaying;
            }
        }

        public void StartSeeking()
        {
            _isSeeking = true;
        }

        public void StopSeeking(double sliderValue)
        {
            _isSeeking = false;
            if (_mediaElementService.MediaElement?.NaturalDuration.HasTimeSpan == true)
            {
                _mediaElementService.MediaElement.Position =
                    TimeSpan.FromSeconds(sliderValue / 100 * _mediaElementService.MediaElement.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        private void StartTimer()
        {
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (_mediaElementService.MediaElement?.NaturalDuration.HasTimeSpan == true)
            {
                Duration = _mediaElementService.MediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss") ?? "00:00";
                CurrentPoint = _mediaElementService.MediaElement.Position.ToString(@"mm\:ss") ?? "00:00";

                if (_isSeeking) return;
                Progress = 100 * _mediaElementService.MediaElement.Position.TotalSeconds / _mediaElementService.MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        [RelayCommand]
        public void Pause() => _mediaElementService.Pause();
        [RelayCommand]
        public void PlayRandomSong() => _mediaElementService.PlayRandomSong();
        [RelayCommand]
        public void Previous() => _mediaElementService.Previous();

        private async Task<Stream> GetAudioStreamFromUriAsync(string uri)
        {
            using HttpClient client = new();
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var audioData = await response.Content.ReadAsByteArrayAsync();
            return new MemoryStream(audioData);
        }
    }
}
