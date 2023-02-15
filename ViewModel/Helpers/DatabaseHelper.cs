using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Radio_Leech.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        private static readonly string dbFile = Path.Combine(Environment.CurrentDirectory, "mvvmDb.db");
        private static readonly string preferences = Path.Combine(Environment.CurrentDirectory, "preferences.db");
        private static readonly string importUrl = "https://github.com/DerekGooding/RPGGamers-Radio-Premium/raw/main/mvvmDb.db?raw=true";

        public static bool Insert<T>(T item) => Insert<T>(item, false);
        public static bool Insert<T>(T item, bool playerPref)
        {
            using SQLiteConnection connection = new(playerPref? preferences : dbFile);
            connection.CreateTable<T>();
            int rows = connection.Insert(item);
            return rows > 0;
        }
        public static bool Update<T>(T item) => Update<T>(item, false);
        public static bool Update<T>(T item, bool playerPref)
        {
            using SQLiteConnection connection = new(playerPref ? preferences : dbFile);
            connection.CreateTable<T>();
            int rows = connection.Update(item);
            return rows > 0;
        }
        public static bool Delete<T>(T item) => Delete<T>(item, false);
        public static bool Delete<T>(T item, bool playerPref)
        {
            using SQLiteConnection connection = new(playerPref ? preferences : dbFile);
            connection.CreateTable<T>();
            int rows = connection.Delete(item);
            return rows > 0;
        }
        public static List<T> Read<T>() where T : new() => Read<T>(false);
        public static List<T> Read<T>(bool playerPref) where T : new()
        {
            using SQLiteConnection connection = new(playerPref ? preferences : dbFile);
            connection.CreateTable<T>();
            List<T> items = connection.Table<T>().ToList();
            
            return items;
        }

        public static async Task ImportFromOnlineAsync()
        {
            using var client = new HttpClient();
            {
                using var response = client.GetStreamAsync(importUrl);
                {
                    using var stream = new FileStream(dbFile, FileMode.Create);
                    {
                        try
                        {
                            await response.Result.CopyToAsync(stream);
                        }
                        catch { }
                    }
                }
            }
        }

    }
}
