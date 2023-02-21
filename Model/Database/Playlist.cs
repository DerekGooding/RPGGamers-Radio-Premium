using SQLite;

namespace Radio_Leech.Model.Database
{
    public class Playlist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string? Name { get; set; }
    }
}
