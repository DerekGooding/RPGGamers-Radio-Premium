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
        private static readonly string importUrl = "https://github.com/DerekGooding/RPGGamers-Radio-Premium/raw/main/mvvmDb.db";

        public static bool Insert<T>(T item)
        {
            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            int rows = connection.Insert(item);
            return rows > 0;
        }
        public static bool Update<T>(T item)
        {
            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            int rows = connection.Update(item);
            return rows > 0;
        }
        public static bool Delete<T>(T item)
        {
            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            int rows = connection.Delete(item);
            return rows > 0;
        }

        public static List<T> Read<T>() where T : new()
        {
            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            List<T> items = connection.Table<T>().ToList();
            return items;
        }

        public static async Task ImportFromOnlineAsync()
        {
            //if (File.Exists(dbFile)) File.Delete(dbFile);
            using var client = new HttpClient();
            {
                using var response = client.GetStreamAsync(importUrl);
                {
                    using var stream = new FileStream(dbFile, FileMode.CreateNew);
                    {
                        await response.Result.CopyToAsync(stream);

                    }
                }
            }

            //using SQLiteConnection connection = new(dbFile);
            //connection.CreateTable<T>();
            //List<T> items = connection.Table<T>().ToList();
        }

    }
}
