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

        public enum Target
        {
            Database,
            UserPrefs
        }
        private static readonly string localAppData = Environment.GetFolderPath(
                                  Environment.SpecialFolder.LocalApplicationData);
        private static readonly string userFilePath = Path.Combine(localAppData, "LibertasInfinitum");

        private static readonly string dbFile = Path.Combine(userFilePath, "mvvmDb.db");
        private static readonly string preferences = Path.Combine(userFilePath, "preferences.db");
        private static readonly string importUrl = "https://github.com/DerekGooding/RPGGamers-Radio-Premium/raw/main/mvvmDb.db?raw=true";

        public static void InitializeFolder()
        {
            if (!Directory.Exists(userFilePath))
                Directory.CreateDirectory(userFilePath);
        }

        public static bool Insert<T>(T item, Target target)
        {
            using SQLiteConnection connection = new(target == Target.UserPrefs ? preferences : dbFile);
            connection.CreateTable<T>();
            int rows = connection.Insert(item);
            return rows > 0;
        }
        public static bool Update<T>(T item, Target target)
        {
            using SQLiteConnection connection = new(target == Target.UserPrefs ? preferences : dbFile);
            connection.CreateTable<T>();
            int rows = connection.Update(item);
            return rows > 0;
        }
        public static bool Delete<T>(T item, Target target)
        {
            using SQLiteConnection connection = new(target == Target.UserPrefs ? preferences : dbFile);
            connection.CreateTable<T>();
            int rows = connection.Delete(item);
            return rows > 0;
        }
        public static List<T> Read<T>(Target target) where T : new()
        {
            using SQLiteConnection connection = new(target == Target.UserPrefs ? preferences : dbFile);
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
