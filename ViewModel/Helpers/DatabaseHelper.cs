using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Radio_Leech.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        private static readonly string dbFile = Path.Combine(Environment.CurrentDirectory, "mvvmDb.db");

        public static bool Insert<T>(T item)
        {
            bool result = false;

            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            int rows = connection.Insert(item);
            if (rows > 0)
                result = true;

            return result;
        }
        public static bool Update<T>(T item)
        {
            bool result = false;

            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            int rows = connection.Update(item);
            if (rows > 0)
                result = true;

            return result;
        }
        public static bool Delete<T>(T item)
        {
            bool result = false;

            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            int rows = connection.Delete(item);
            if (rows > 0)
                result = true;

            return result;
        }

        public static List<T> Read<T>() where T : new()
        {
            List<T> items;

            using SQLiteConnection connection = new(dbFile);
            connection.CreateTable<T>();
            items = connection.Table<T>().ToList();

            return items;
        }

    }
}
