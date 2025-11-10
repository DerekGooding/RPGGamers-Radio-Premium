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

[Singleton]
public class MediaElementService : IDisposable
{
    private readonly DatabaseService _databaseService;
    private readonly NotificationService _notificationService;
    private readonly PreferencesService _preferencesService;
    private readonly Dictionary<string, BitmapImage> _imageCache = [];
    private readonly HttpClient _httpClient;

    private const int SongHistoryMax = 50;
    private const string BaseUrl = "https://raw.githubusercontent.com";
    private const string GitHubRepo = "DerekGooding/SongBackup";
    private const string GitHubBranch = "master";

    private Stack<SongImage> _songHistory = [];
    private bool _isPlaying;
    private SongImage _currentlyPlaying = new();
    private bool _disposed;

    public MediaElementService(
        DatabaseService databaseService,
        NotificationService notificationService,
        PreferencesService preferenceService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _preferencesService = preferenceService ?? throw new ArgumentNullException(nameof(preferenceService));

        // Initialize HttpClient once for reuse
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("song-player", "1.0"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        InitializeSongImages();
    }

    public IWavePlayer OutputDevice { get; private set; } = new WaveOutEvent();
    public Mp3FileReader? MP3Reader { get; private set; }
    public List<SongImage> SongImages { get; } = [];

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (_isPlaying != value)
            {
                _isPlaying = value;
                PlayStatusChange?.Invoke(value, EventArgs.Empty);
            }
        }
    }

    public SongImage CurrentlyPlaying
    {
        get => _currentlyPlaying;
        set
        {
            if (_currentlyPlaying != value)
            {
                _currentlyPlaying = value;
                SongChange?.Invoke(value, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? SongChange;
    public event EventHandler? PlayStatusChange;

    private void InitializeSongImages()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var imageSources = assembly.GetManifestResourceNames();
        var imageSourceSet = new HashSet<string>(imageSources);
        var fallbackSource = imageSourceSet.FirstOrDefault(name => name.Contains("ComingSoon.jpg"))
            ?? throw new InvalidOperationException("Fallback image 'ComingSoon.jpg' not found in resources.");

        SongImages.AddRange(
            _databaseService.Read()
                .Select(song => new SongImage(
                    song,
                    GetCachedImage(song, imageSourceSet, assembly, fallbackSource),
                    false,
                    false))
        );
    }

    public async Task PlayMediaAsync(SongImage songImage, bool isPrevious = false)
    {
        ArgumentNullException.ThrowIfNull(songImage);

        // Add to history if not navigating backwards
        if (!isPrevious &&
            (_songHistory.Count == 0 || _songHistory.Peek() != CurrentlyPlaying) &&
            !string.Equals(CurrentlyPlaying.Song.Game, "None", StringComparison.OrdinalIgnoreCase))
        {
            _songHistory.Push(CurrentlyPlaying);

            // Trim history if needed
            if (_songHistory.Count > SongHistoryMax)
            {
                _songHistory = new Stack<SongImage>(_songHistory.Take(SongHistoryMax));
            }
        }

        // Show notification without awaiting (fire and forget with proper error handling)
        _ = Task.Run(async () =>
        {
            try
            {
                await _notificationService.ShowNotificationAsync(songImage.Song.Game, songImage.Song.Title);
            }
            catch (Exception ex)
            {
                // Log error but don't block playback
                System.Diagnostics.Debug.WriteLine($"Notification failed: {ex.Message}");
            }
        });

        await DownloadAndPlaySongAsync(songImage);

        CurrentlyPlaying = songImage;
        IsPlaying = true;
    }

    private void PlayMp3Bytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            throw new ArgumentException("Invalid audio data.", nameof(bytes));

        // Clean up previous playback
        CleanupPlayback();

        var memoryStream = new MemoryStream(bytes);
        MP3Reader = new Mp3FileReader(memoryStream);

        OutputDevice = new WaveOutEvent();
        OutputDevice.Init(MP3Reader);
        OutputDevice.PlaybackStopped += Element_MediaEnded;
        OutputDevice.Play();
    }

    private void CleanupPlayback()
    {
        if (OutputDevice != null)
        {
            OutputDevice.PlaybackStopped -= Element_MediaEnded;
            OutputDevice.Stop();
            OutputDevice.Dispose();
        }

        MP3Reader?.Dispose();
    }

    private async Task DownloadAndPlaySongAsync(SongImage songImage)
    {
        var id = songImage.Song.Id.ToString().PadLeft(4, '0');
        var path = $"SongBackup/Songs/{id[0]}000/{id}.mp3";

        try
        {
            await DownloadSongAsync(path);
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Download failed: {ex.Message}");
            HandleAuthenticationFailure();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    private void HandleAuthenticationFailure()
    {
        var dlg = new InputTextWindow(_preferencesService.LoadAPI());
        if (dlg.ShowDialog() == true)
        {
            _preferencesService.SaveAPI(dlg.Result);
        }
    }

    private async Task DownloadSongAsync(string path)
    {
        var api = _preferencesService.LoadAPI();

        // Update authorization header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api);

        var url = $"{BaseUrl}/{GitHubRepo}/{GitHubBranch}/{Uri.EscapeDataString(path)}";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        PlayMp3Bytes(bytes);
    }

    private void Element_MediaEnded(object? sender, StoppedEventArgs e)
    {
        // Check if stopped due to end of track (not user action or error)
        if (e.Exception == null)
        {
            PlayRandomSong();
        }
    }

    public async void PlayRandomSong()
    {
        try
        {
            var playableSongs = SongImages.Where(si => !si.IsIgnored).ToList();

            if (playableSongs.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No playable songs available.");
                return;
            }

            var randomSong = playableSongs[Random.Shared.Next(playableSongs.Count)];
            await PlayMediaAsync(randomSong);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to play random song: {ex.Message}");
        }
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
            if (string.Equals(CurrentlyPlaying.Song.Game, "None", StringComparison.OrdinalIgnoreCase))
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

    public async void Previous()
    {
        if (_songHistory.Count == 0)
            return;

        try
        {
            await PlayMediaAsync(_songHistory.Pop(), isPrevious: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to play previous song: {ex.Message}");
        }
    }

    private BitmapImage GetCachedImage(Song song, HashSet<string> imageSourceSet, Assembly assembly, string fallbackSource)
    {
        var sanitizedGameName = SanitizeFileName(song.Game);
        var resourceName = imageSourceSet.FirstOrDefault(name => name.Contains($"{sanitizedGameName}.jpg"))
                          ?? fallbackSource;

        // Return from cache if already loaded
        if (_imageCache.TryGetValue(resourceName, out var cachedImage))
            return cachedImage;

        // Load and cache the image
        using var stream = assembly.GetManifestResourceStream(resourceName)
                          ?? throw new FileNotFoundException($"Resource '{resourceName}' not found.");

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = stream;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze(); // Important for thread safety and performance

        _imageCache[resourceName] = bitmap;
        return bitmap;
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = new[] { ':', '/', '<', '>', '"', '\\', '|', '?', '*' };
        return invalidChars.Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            CleanupPlayback();
            _httpClient?.Dispose();

            // Clear image cache
            _imageCache.Clear();
        }

        _disposed = true;
    }
}