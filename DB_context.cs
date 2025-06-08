using Microsoft.Data.Sqlite;
using System;

namespace MyCoreApp
{
    public class DB_context : IDisposable
    {
        private readonly SqliteConnection _connection;

        public DB_context(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            CreateUsersTable();
        }

        public string? GetPasswordHash(string username)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT Password FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);

            return command.ExecuteScalar()?.ToString();
        }

        private void CreateUsersTable()
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    UserType TEXT NOT NULL
                )";
            command.ExecuteNonQuery();
        }

        public bool RegisterUser(string username, string password, string email, string userType)
        {
            if (UserExists(username) || EmailExists(email))
                return false;

            var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Username, Password, Email, UserType) 
                VALUES ($username, $password, $email, $userType)";

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", BCrypt.Net.BCrypt.HashPassword(password));
            command.Parameters.AddWithValue("$email", email);
            command.Parameters.AddWithValue("$userType", userType);

            return command.ExecuteNonQuery() > 0;
        }

        public bool EmailExists(string email)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM Users WHERE Email = $email";
            command.Parameters.AddWithValue("$email", email);
            return command.ExecuteScalar() != null;
        }

        public bool ValidateUser(string username, string password)
        {
            var hashedPassword = GetPasswordHash(username);
            return hashedPassword != null && BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public bool UserExists(string username)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);

            return command.ExecuteScalar() != null;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
