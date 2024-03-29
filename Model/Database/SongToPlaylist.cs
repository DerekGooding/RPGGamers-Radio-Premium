﻿using SQLite;

namespace Radio_Leech.Model.Database
{
    public class SongToPlaylist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int PlaylistID { get; set; }
        [NotNull]
        public int SongID { get; set; }
    }
}
