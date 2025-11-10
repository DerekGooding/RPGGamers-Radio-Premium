using System.IO;

namespace GamerRadio.Services;

[Singleton]
public class PreferencesService
{
    public PreferencesService()
    {
        _appFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RPGGamerRadio");
        _preferencesFilePath = Path.Combine(_appFolderPath, "preferences.csv");
        _aPIKeyFilePath = Path.Combine(_appFolderPath, "apikey.txt");
        TempMp3 = Path.Combine(_appFolderPath, "temp.mp3");
    }
    private readonly string _appFolderPath;
    private readonly string _preferencesFilePath;
    private readonly string _aPIKeyFilePath;
    public readonly string TempMp3;
    private const char _separator = '|';

    private string _aPIKey = string.Empty;

    public void SavePreferences(string[] preferences)
    {
        if (!Directory.Exists(_appFolderPath))
        {
            Directory.CreateDirectory(_appFolderPath);
        }

        File.WriteAllLines(_preferencesFilePath, preferences);
    }
    public string LoadAPI()
    {
        if (_aPIKey.Length == 0 && File.Exists(_aPIKeyFilePath))
            _aPIKey = File.ReadAllText(_aPIKeyFilePath);
        return _aPIKey;
    }

    public void SaveAPI(string apiKey)
    {
        _aPIKey = apiKey;
        File.WriteAllText(_aPIKeyFilePath, _aPIKey);
    }

    public string[] LoadPreferences() => File.Exists(_preferencesFilePath) ? File.ReadAllLines(_preferencesFilePath) : [];

    public void Save(bool MinToTray, bool NotificationOn, int NotificationCorner, float Volume, IEnumerable<int> Favorites, IEnumerable<int> Blocked)
    {
        List<string> data = [];
        data.Add($"MinToTray{_separator}{MinToTray}");
        data.Add($"NotificationOn{_separator}{NotificationOn}");
        data.Add($"NotificationCorner{_separator}{NotificationCorner}");
        data.Add($"Volume{_separator}{Volume}");
        data.AddRange(Favorites.Select(x => $"Favorites{_separator}{x}"));
        data.AddRange(Blocked.Select(x => $"Blocked{_separator}{x}"));
        SavePreferences([.. data]);
    }
    public  (bool MinToTray, bool NotificationOn, int NotificationCorner, float Volume, List<int> Favorites, List<int> Blocked) Load()
    {
        var preferences = LoadPreferences();
        var MinToTray = true;
        var NotificationOn = true;
        var NotificationCorner = 0;
        var Volume = 0.5f;
        List<int> Favorites = [];
        List<int> Blocked = [];
        foreach (var line in preferences.Select(x => x.Split(_separator)))
        {
            if (line[0] == "NotificationOn" && bool.TryParse(line[1], out var notificationOn))
                NotificationOn = notificationOn;
            else if (line[0] == "NotificationCorner" && int.TryParse(line[1], out var notificationCorner))
                NotificationCorner = notificationCorner;
            else if (line[0] == "Volume" && float.TryParse(line[1], out var volume))
                Volume = volume;
            else if (line[0] == "Favorites" && int.TryParse(line[1], out var favorite))
                Favorites.Add(favorite);
            else if (line[0] == "Blocked" && int.TryParse(line[1], out var blocked))
                Blocked.Add(blocked);
            else if (line[0] == "MinToTray" && bool.TryParse(line[1], out var minToTray))
                MinToTray = minToTray;
        }
        return (MinToTray, NotificationOn, NotificationCorner, Volume, Favorites, Blocked);
    }
}
