using GamerRadio.Model;
using System.IO;

namespace GamerRadio.Services;

[Singleton]
public class DatabaseService
{
    private const char _separator = '|';

    private readonly Uri _database = new("Assets\\Links\\database.csv", UriKind.Relative);

    public List<Song> Read()
    {
        using var stream = Application.GetContentStream(_database).Stream;
        using StreamReader reader = new(stream);
        return [.. reader
            .ReadToEnd()
            .Split(Environment.NewLine)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(LineToSong)];
    }

    private Song LineToSong(string line)
    {
        var parts = line.Split(_separator);
        return new Song(int.Parse(parts[0]), parts[1], parts[2], parts[3]);
    }
}
