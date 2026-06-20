using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MovieTracker
{
    public class Database
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "movies.db");
        private static string connStr = $"Data Source={dbPath};Version=3;";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Movies (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Type TEXT,
                    Status TEXT,
                    Rating INTEGER,
                    Genre TEXT,
                    Notes TEXT,
                    CoverUrl TEXT,
                    AddedDate TEXT,
                    WatchDate TEXT,
                    Reminder INTEGER DEFAULT 0
                )";
                new SQLiteCommand(sql, conn).ExecuteNonQuery();

                // 如果舊資料庫沒有新欄位，自動加上
                try { new SQLiteCommand("ALTER TABLE Movies ADD COLUMN WatchDate TEXT", conn).ExecuteNonQuery(); } catch { }
                try { new SQLiteCommand("ALTER TABLE Movies ADD COLUMN Reminder INTEGER DEFAULT 0", conn).ExecuteNonQuery(); } catch { }
            }
        }

        public static List<Movie> GetAll()
        {
            var list = new List<Movie>();
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Movies ORDER BY Id DESC", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Movie
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Title = reader["Title"].ToString(),
                        Type = reader["Type"].ToString(),
                        Status = reader["Status"].ToString(),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Genre = reader["Genre"].ToString(),
                        Notes = reader["Notes"].ToString(),
                        CoverUrl = reader["CoverUrl"].ToString(),
                        AddedDate = reader["AddedDate"].ToString(),
                        WatchDate = reader["WatchDate"].ToString(),
                        Reminder = Convert.ToInt32(reader["Reminder"]) == 1
                    });
                }
            }
            return list;
        }

        public static void Add(Movie m)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = "INSERT INTO Movies (Title,Type,Status,Rating,Genre,Notes,CoverUrl,AddedDate,WatchDate,Reminder) VALUES (@t,@ty,@s,@r,@g,@n,@c,@d,@w,@rem)";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@t", m.Title);
                cmd.Parameters.AddWithValue("@ty", m.Type);
                cmd.Parameters.AddWithValue("@s", m.Status);
                cmd.Parameters.AddWithValue("@r", m.Rating);
                cmd.Parameters.AddWithValue("@g", m.Genre);
                cmd.Parameters.AddWithValue("@n", m.Notes);
                cmd.Parameters.AddWithValue("@c", m.CoverUrl);
                cmd.Parameters.AddWithValue("@d", m.AddedDate);
                cmd.Parameters.AddWithValue("@w", m.WatchDate);
                cmd.Parameters.AddWithValue("@rem", m.Reminder ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Update(Movie m)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = "UPDATE Movies SET Title=@t,Type=@ty,Status=@s,Rating=@r,Genre=@g,Notes=@n,CoverUrl=@c,WatchDate=@w,Reminder=@rem WHERE Id=@id";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@t", m.Title);
                cmd.Parameters.AddWithValue("@ty", m.Type);
                cmd.Parameters.AddWithValue("@s", m.Status);
                cmd.Parameters.AddWithValue("@r", m.Rating);
                cmd.Parameters.AddWithValue("@g", m.Genre);
                cmd.Parameters.AddWithValue("@n", m.Notes);
                cmd.Parameters.AddWithValue("@c", m.CoverUrl);
                cmd.Parameters.AddWithValue("@w", m.WatchDate);
                cmd.Parameters.AddWithValue("@rem", m.Reminder ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", m.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Delete(int id)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                var cmd = new SQLiteCommand("DELETE FROM Movies WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Movie> Search(string keyword, string type, string status, string genre)
        {
            var list = new List<Movie>();
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Movies WHERE 1=1";
                if (!string.IsNullOrEmpty(keyword)) sql += " AND Title LIKE @k";
                if (type != "全部") sql += " AND Type=@ty";
                if (status != "全部") sql += " AND Status=@s";
                if (genre != "全部") sql += " AND Genre=@g";
                sql += " ORDER BY Id DESC";

                var cmd = new SQLiteCommand(sql, conn);
                if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@k", $"%{keyword}%");
                if (type != "全部") cmd.Parameters.AddWithValue("@ty", type);
                if (status != "全部") cmd.Parameters.AddWithValue("@s", status);
                if (genre != "全部") cmd.Parameters.AddWithValue("@g", genre);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Movie
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Title = reader["Title"].ToString(),
                        Type = reader["Type"].ToString(),
                        Status = reader["Status"].ToString(),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Genre = reader["Genre"].ToString(),
                        Notes = reader["Notes"].ToString(),
                        CoverUrl = reader["CoverUrl"].ToString(),
                        AddedDate = reader["AddedDate"].ToString(),
                        WatchDate = reader["WatchDate"].ToString(),
                        Reminder = Convert.ToInt32(reader["Reminder"]) == 1
                    });
                }
            }
            return list;
        }
    }
}