using GamerRadio.Model;
using System.IO;

namespace GamerRadio.Services;

public class DatabaseService
{
    private const char separator = '|';

    private readonly Uri _database = new("Assets\\Links\\database.csv", UriKind.Relative);

    public List<Song> Read()
    {
        using Stream stream = Application.GetContentStream(_database).Stream;
        using StreamReader reader = new(stream);
        return [.. reader
            .ReadToEnd()
            .Split(Environment.NewLine)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(LineToSong)];
    }

    private Song LineToSong(string line)
    {
        string[] parts = line.Split(separator);
        return new Song(int.Parse(parts[0]), parts[1], parts[2], parts[3]);
    }
}
