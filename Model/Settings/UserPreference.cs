using SQLite;

namespace Radio_Leech.Model.Settings
{
    public class UserPreference
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string? Name { get; set; }
        public bool? IsTrue { get; set; }
        public int? Value { get; set; }
        public double? Percent { get; set; }
    }
}
