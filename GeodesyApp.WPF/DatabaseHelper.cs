using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace GeodesyApp
{
    public class DatabaseHelper
    {
        private string _connectionString;

        // Конструктор принимает путь к файлу базы данных
        public DatabaseHelper(string dbFilePath)
        {
            _connectionString = $"Data Source={dbFilePath}";

            // Создаём базу данных и таблицу, если их нет
            InitializeDatabase();
        }

        // Создаёт файл базы и таблицу Register при первом запуске
        private void InitializeDatabase()
        {
            using(var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Register (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Название TEXT NOT NULL,
                        Оси TEXT NOT NULL,
                        Отметка TEXT NOT NULL,
                        Объем REAL,
                        Статус TEXT NOT NULL DEFAULT 'Новая'
                    )";

                using(var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получить все схемы
        public List<Scheme> GetAllSchemes()
        {
            var schemes = new List<Scheme>();

            using(var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT id, Название, Оси, Отметка, Объем, Статус FROM Register ORDER BY id";

                using(var command = new SqliteCommand(query, connection))
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var scheme = new Scheme
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Axes = reader.GetString(2),
                            Mark = reader.GetString(3),
                            Volume = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            Status = reader.GetString(5)
                        };
                        schemes.Add(scheme);
                    }
                }
            }

            return schemes;
        }

        // Добавить новую схему
        public bool AddScheme(Scheme scheme)
        {
            string query = @"INSERT INTO Register (Название, Оси, Отметка, Объем, Статус) 
                           VALUES (@name, @axes, @mark, @volume, @status)";

            try
            {
                using(var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using(var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", scheme.Name);
                        command.Parameters.AddWithValue("@axes", scheme.Axes);
                        command.Parameters.AddWithValue("@mark", scheme.Mark);

                        if(scheme.Volume.HasValue)
                            command.Parameters.AddWithValue("@volume", scheme.Volume.Value);
                        else
                            command.Parameters.AddWithValue("@volume", DBNull.Value);

                        command.Parameters.AddWithValue("@status", scheme.Status);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
                return false;
            }
        }

        // Обновить все поля схемы
        public bool UpdateScheme(Scheme scheme)
        {
            string query = @"UPDATE Register 
                           SET Название = @name, 
                               Оси = @axes, 
                               Отметка = @mark, 
                               Объем = @volume, 
                               Статус = @status 
                           WHERE id = @id";

            try
            {
                using(var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using(var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", scheme.Id);
                        command.Parameters.AddWithValue("@name", scheme.Name);
                        command.Parameters.AddWithValue("@axes", scheme.Axes);
                        command.Parameters.AddWithValue("@mark", scheme.Mark);

                        if(scheme.Volume.HasValue)
                            command.Parameters.AddWithValue("@volume", scheme.Volume.Value);
                        else
                            command.Parameters.AddWithValue("@volume", DBNull.Value);

                        command.Parameters.AddWithValue("@status", scheme.Status);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении: {ex.Message}");
                return false;
            }
        }

        // Удалить схему по ID
        public bool DeleteScheme(int id)
        {
            string query = "DELETE FROM Register WHERE id = @id";

            try
            {
                using(var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using(var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
                return false;
            }
        }
    }
}