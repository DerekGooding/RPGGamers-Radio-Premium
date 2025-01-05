using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GamerRadio.Model;

namespace GamerRadio.Services;

public class MediaElementService
{
    private readonly DatabaseService _databaseService;
    private readonly NotificationService _notificationService;
    private readonly Dictionary<string, BitmapImage> _imageCache = [];

    public MediaElementService(DatabaseService databaseService, NotificationService notificationService)
    {
        _databaseService = databaseService;
        _notificationService = notificationService;

        Assembly assembly = Assembly.GetExecutingAssembly();
        string[] _imageSources = assembly.GetManifestResourceNames();
        HashSet<string> imageSourceSet = new(_imageSources);

        SongImages = _databaseService.Read()
                .ConvertAll(song => new SongImage(song, GetCachedImage(song, imageSourceSet, assembly, _imageSources)));
    }

    public MediaElement? MediaElement { get; set; }

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
        get { return currentlyPlaying; }
        set
        {
            currentlyPlaying = value;
            SongChange?.Invoke(value, EventArgs.Empty);
        }
    }

    private SongImage currentlyPlaying = new() { Song = new() { Game = "None", Title = "None" } };

    public EventHandler? SongChange;

    public EventHandler? PlayStatusChange;

    private bool _subscribed;


    public void PlayMedia(SongImage songImage, bool isPrevious = false)
    {
        if (MediaElement is not MediaElement mediaElement) return;
        if (!_subscribed)
        {
            mediaElement.MediaEnded += Element_MediaEnded;
            _subscribed = true;
        }

        if (!isPrevious && (_songHistory.Count == 0 || _songHistory.Peek() != CurrentlyPlaying) && CurrentlyPlaying.Song.Game != "None")
        {
            _songHistory.Push(CurrentlyPlaying);
            if (_songHistory.Count > SongHistoryMax)
                _songHistory = new Stack<SongImage>(_songHistory.Take(SongHistoryMax));
        }

        new Thread(async () => await _notificationService.ShowNotificationAsync(songImage.Song.Game, songImage.Song.Title)).Start();

        mediaElement.Source = new(songImage.Song.Url);
        mediaElement.Play();

        CurrentlyPlaying = songImage;

        IsPlaying = true;
    }

    private void Element_MediaEnded(object sender, RoutedEventArgs e) => PlayRandomSong();

    public void PlayRandomSong() => PlayMedia(SongImages[Random.Shared.Next(SongImages.Count - 1)]);

    public void Pause()
    {
        if (MediaElement is not MediaElement mediaElement) return;
        if (IsPlaying)
        {
            mediaElement.Pause();
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
                mediaElement.Play();
                IsPlaying = true;
            }
        }
    }

    public void Previous()
    {
        if (_songHistory.Count == 0) return;
        PlayMedia(_songHistory.Pop(), true);
    }

    private ImageSource GetCachedImage(Song song, HashSet<string> imageSourceSet, Assembly assembly, string[] fallbackSources)
    {
        string resourceName = imageSourceSet.FirstOrDefault(name => name.Contains($"{song.Game}.jpg"))
                              ?? fallbackSources[1];

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
