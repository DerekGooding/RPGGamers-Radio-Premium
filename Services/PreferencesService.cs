using System.IO;

namespace GamerRadio.Services;

public class PreferencesService
{
    public PreferencesService()
    {
        AppFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RPGGamerRadio");
        PreferencesFilePath = Path.Combine(AppFolderPath, "preferences.csv");
        APIKeyFilePath = Path.Combine(AppFolderPath, "apikey.txt");
        TempMp3 = Path.Combine(AppFolderPath, "temp.mp3");
    }
    private readonly string AppFolderPath;
    private readonly string PreferencesFilePath;
    private readonly string APIKeyFilePath;
    public readonly string TempMp3;
    private const char separator = '|';

    private string APIKey = string.Empty;

    public void SavePreferences(string[] preferences)
    {
        if (!Directory.Exists(AppFolderPath))
        {
            Directory.CreateDirectory(AppFolderPath);
        }

        File.WriteAllLines(PreferencesFilePath, preferences);
    }
    public string LoadAPI()
    {
        if (APIKey.Length == 0 && File.Exists(APIKeyFilePath))
            APIKey = File.ReadAllText(APIKeyFilePath);
        return APIKey;
    }

    public void SaveAPI(string apiKey)
    {
        APIKey = apiKey;
        File.WriteAllText(APIKeyFilePath, APIKey);
    }

    public string[] LoadPreferences() => File.Exists(PreferencesFilePath) ? File.ReadAllLines(PreferencesFilePath) : [];

    public void Save(bool MinToTray, bool NotificationOn, int NotificationCorner, float Volume, IEnumerable<int> Favorites, IEnumerable<int> Blocked)
    {
        List<string> data = [];
        data.Add($"MinToTray{separator}{MinToTray}");
        data.Add($"NotificationOn{separator}{NotificationOn}");
        data.Add($"NotificationCorner{separator}{NotificationCorner}");
        data.Add($"Volume{separator}{Volume}");
        data.AddRange(Favorites.Select(x => $"Favorites{separator}{x}"));
        data.AddRange(Blocked.Select(x => $"Blocked{separator}{x}"));
        SavePreferences([.. data]);
    }
    public  (bool MinToTray, bool NotificationOn, int NotificationCorner, float Volume, List<int> Favorites, List<int> Blocked) Load()
    {
        string[] preferences = LoadPreferences();
        bool MinToTray = true;
        bool NotificationOn = true;
        int NotificationCorner = 0;
        float Volume = 0.5f;
        List<int> Favorites = [];
        List<int> Blocked = [];
        foreach (string[] line in preferences.Select(x => x.Split(separator)))
        {
            if (line[0] == "NotificationOn" && bool.TryParse(line[1], out bool notificationOn))
                NotificationOn = notificationOn;
            else if (line[0] == "NotificationCorner" && int.TryParse(line[1], out int notificationCorner))
                NotificationCorner = notificationCorner;
            else if (line[0] == "Volume" && float.TryParse(line[1], out float volume))
                Volume = volume;
            else if (line[0] == "Favorites" && int.TryParse(line[1], out int favorite))
                Favorites.Add(favorite);
            else if (line[0] == "Blocked" && int.TryParse(line[1], out int blocked))
                Blocked.Add(blocked);
            else if (line[0] == "MinToTray" && bool.TryParse(line[1], out bool minToTray))
                MinToTray = minToTray;
        }
        return (MinToTray, NotificationOn, NotificationCorner, Volume, Favorites, Blocked);
    }
}
