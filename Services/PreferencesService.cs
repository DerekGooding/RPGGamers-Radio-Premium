using System.IO;

namespace GamerRadio.Services;

public class PreferencesService
{
    private static readonly string AppFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RPGGamerRadio");

    private static readonly string PreferencesFilePath = Path.Combine(AppFolderPath, "preferences.csv");

    public static void SavePreferences(string[] preferences)
    {
        if (!Directory.Exists(AppFolderPath))
        {
            Directory.CreateDirectory(AppFolderPath);
        }

        File.WriteAllLines(PreferencesFilePath, preferences);
    }

    public static string[] LoadPreferences()
    {
        if (File.Exists(PreferencesFilePath))
        {
            return File.ReadAllLines(PreferencesFilePath);
        }

        return [];
    }
}
