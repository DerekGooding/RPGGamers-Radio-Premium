using GamerRadio.Model;
using GamerRadio.Services;
using System.Windows.Media;
using System.Windows.Threading;

namespace GamerRadio.ViewModel.Pages;

[ViewModel,Singleton]
public partial class DashboardViewModel
{
    private readonly MediaElementService _mediaElementService;

    private bool _isSeeking;
    private readonly DispatcherTimer _timer;

    [Bind] private bool _isPlaying;
    [Bind] private string _duration = "00:00";
    [Bind] private string _currentPoint = "00:00";
    [Bind] private double _progress;
    [Bind] private SongImage _currentlyPlaying = new();
    [Bind] private ImageSource? _wavRender;
    [Bind(OnChangeMethodName = nameof(OnVolumeChanged))] private float _volume = 1.0f;
    public void OnVolumeChanged() => _mediaElementService.OutputDevice.Volume = _volume;

    public DashboardViewModel(MediaElementService mediaElementService)
    {
        _mediaElementService = mediaElementService;
        _mediaElementService.SongChange += HandleSongChange;
        _mediaElementService.PlayStatusChange += HandlePlayStatusChange;

        _timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(20)
        };
        _timer.Tick += TimerTick;
        _timer.Start();
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

    public void StartSeeking() => _isSeeking = true;

    public void StopSeeking(double sliderValue)
    {
        _isSeeking = false;
        if (_mediaElementService.MP3Reader != null)
        {
            _mediaElementService.MP3Reader.CurrentTime =
                TimeSpan.FromSeconds(sliderValue / 100 * _mediaElementService.MP3Reader.TotalTime.TotalSeconds);
        }
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        if (_mediaElementService.MP3Reader != null)
        {
            Duration = _mediaElementService.MP3Reader.TotalTime.ToString(@"mm\:ss") ?? "00:00";
            CurrentPoint = _mediaElementService.MP3Reader.CurrentTime.ToString(@"mm\:ss") ?? "00:00";

            if (_isSeeking) return;
            Progress = 100 * _mediaElementService.MP3Reader.CurrentTime.TotalSeconds / _mediaElementService.MP3Reader.TotalTime.TotalSeconds;
        }
    }

    [Command]
    public void DashboardPause() => _mediaElementService.Pause();
    [Command]
    public void DashboardPlayRandomSong() => _mediaElementService.PlayRandomSong();
    [Command]
    public void DashboardPrevious() => _mediaElementService.Previous();
}
