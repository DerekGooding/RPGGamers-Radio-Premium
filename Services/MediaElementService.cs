using GamerRadio.Model;
using GamerRadio.ViewModel.Windows;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.Wave;

namespace GamerRadio.Services;

public class MediaElementService
{
    private readonly DatabaseService _databaseService;
    private readonly NotificationService _notificationService;
    private readonly PreferencesService _preferencesService;
    private readonly Dictionary<string, BitmapImage> _imageCache = [];

    public MediaElementService(DatabaseService databaseService, NotificationService notificationService, PreferencesService preferenceService)
    {
        _databaseService = databaseService;
        _notificationService = notificationService;
        _preferencesService = preferenceService;

        Assembly assembly = Assembly.GetExecutingAssembly();
        string[] _imageSources = assembly.GetManifestResourceNames();
        HashSet<string> imageSourceSet = [.. _imageSources];
        string fallbackSource = imageSourceSet.FirstOrDefault(name => name.Contains("ComingSoon.jpg"))!;
        SongImages = _databaseService.Read()
                .ConvertAll(song => new SongImage(song, GetCachedImage(song, imageSourceSet, assembly, fallbackSource), false, false));
    }

    //public MediaElement? MediaElement { get; set; }
    public WaveOutEvent OutputDevice { get; private set; } = new();
    private Mp3FileReader? mp3Reader;


    public List<SongImage> SongImages { get; } = [];

    private const int SongHistoryMax = 50;
    private Stack<SongImage> _songHistory = [];

    private bool isPlaying;
    public bool IsPlaying
    {
        get => isPlaying; set
        {
            isPlaying = value;
            PlayStatusChange?.Invoke(value, EventArgs.Empty);
        }
    }
    public SongImage CurrentlyPlaying
    {
        get => currentlyPlaying;
        set
        {
            currentlyPlaying = value;
            SongChange?.Invoke(value, EventArgs.Empty);
        }
    }

    private SongImage currentlyPlaying = new();

    public EventHandler? SongChange;

    public EventHandler? PlayStatusChange;

    private bool _subscribed;


    public async Task PlayMedia(SongImage songImage, bool isPrevious = false)
    {
        if (!_subscribed)
        {
            OutputDevice.PlaybackStopped += Element_MediaEnded;
            _subscribed = true;
        }

        if (!isPrevious && (_songHistory.Count == 0 || _songHistory.Peek() != CurrentlyPlaying) && CurrentlyPlaying.Song.Game != "None")
        {
            _songHistory.Push(CurrentlyPlaying);
            if (_songHistory.Count > SongHistoryMax)
                _songHistory = new Stack<SongImage>(_songHistory.Take(SongHistoryMax));
        }

        Task.Run(() => _notificationService.ShowNotificationAsync(songImage.Song.Game, songImage.Song.Title));

        await BuildURL(songImage);

        CurrentlyPlaying = songImage;

        IsPlaying = true;
    }

    private void PlayMp3Bytes(byte[] bytes)
    {
        // Dispose old playback if it exists
        OutputDevice?.Stop();
        OutputDevice?.Dispose();
        mp3Reader?.Dispose();

        var mem = new MemoryStream(bytes);

        mp3Reader = new Mp3FileReader(mem);

        OutputDevice = new WaveOutEvent();
        OutputDevice.Init(mp3Reader);
        OutputDevice.Play();
    }

    private async Task<Uri> BuildURL(SongImage songImage)
    {
        var id = songImage.Song.Id.ToString().PadLeft(4, '0');
        var  path = $"SongBackup/Songs/{id[0]}000/{id}.mp3";

        try
        {
            await DownloadSongTemp(path);
        }
        catch (Exception ex)
        {
            AttemptNewAPIkey();
        }

        return new Uri(_preferencesService.TempMp3);
    }
    private void AttemptNewAPIkey()
    {
        var dlg = new InputTextWindow(_preferencesService.LoadAPI());
        var ok = dlg.ShowDialog() == true;
        var value = ok ? dlg.Result : string.Empty;
        _preferencesService.SaveAPI(value);
    }

    private async Task DownloadSongTemp(string path)
    {
        const string BaseUrl = "https://raw.githubusercontent.com";
        var api = _preferencesService.LoadAPI();

        using var client = new HttpClient();

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("song-player", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        var url = $"{BaseUrl}/DerekGooding/SongBackup/master/{Uri.EscapeDataString(path)}";

        using var res = await client.GetAsync(url);
        res.EnsureSuccessStatusCode();

        var bytes = await res.Content.ReadAsByteArrayAsync();

        PlayMp3Bytes(bytes);
        //await File.WriteAllBytesAsync(_preferencesService.TempMp3, bytes);
    }

    private void Element_MediaEnded(object? sender, StoppedEventArgs e) => PlayRandomSong();

    public void PlayRandomSong()
    {
        var playable = SongImages.Where(songImage => !songImage.IsIgnored).ToList();

        PlayMedia(playable[Random.Shared.Next(playable.Count - 1)]);
    }

    public void Pause()
    {
        if (IsPlaying)
        {
            OutputDevice.Pause();
            IsPlaying = false;
        }
        else
        {
            if (CurrentlyPlaying.Song.Game == "None")
            {
                PlayRandomSong();
            }
            else
            {
                OutputDevice.Play();
                IsPlaying = true;
            }
        }
    }

    public void Previous()
    {
        if (_songHistory.Count == 0) return;
        PlayMedia(_songHistory.Pop(), true);
    }

    private ImageSource GetCachedImage(Song song, HashSet<string> imageSourceSet, Assembly assembly, string fallbackSource)
    {
        string resourceName = imageSourceSet.FirstOrDefault(name => name.Contains($"{song
            .Game
            .Replace(":","")
            .Replace("/", "")
            .Replace("<", "")
            .Replace(">", "")}.jpg"))
                              ?? fallbackSource;

        // Return from cache if already loaded
        if (_imageCache.TryGetValue(resourceName, out BitmapImage? cachedImage))
            return cachedImage;

        // Load the image if not in cache
        using Stream stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly.");

        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.StreamSource = stream;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();

        // Cache the loaded image
        _imageCache[resourceName] = bitmap;

        return bitmap;
    }
}
