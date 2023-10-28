using MonsterCardGame.Models;
using MonsterCardGame.Uitilities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Repositories
{
    internal static class UserRepository
    {
        public static User Create(string username, string password_hash, int coins)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "INSERT INTO users (username, password, coins) VALUES (@username, @password, @coins); SELECT SCOPE_IDENTITY();";

            RepositoryUtilities.AddParameter(command, "username", DbType.String, username);
            RepositoryUtilities.AddParameter(command, "password_hash", DbType.String, password_hash);
            RepositoryUtilities.AddParameter(command, "coins", DbType.Int32, coins);

            int userId = Convert.ToInt32(command.ExecuteScalar());
            User? user = Select(userId);

            if (user == null)
                throw new Exception($"[{nameof(UserRepository)}] Couldn't fetch inserted User");

            return user;
        }

        public static User? Select(int id)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT user_id, username, coins, elo, wins, losses FROM users WHERE user_id = @id";

            RepositoryUtilities.AddParameter(command, "id", DbType.Int32, id);

            IDataReader reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            return new User()
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Coins = reader.GetInt32(2),
                Elo = reader.GetInt32(3),
                Wins = reader.GetInt32(4),
                Losses = reader.GetInt32(5),
            };
        }

        public static User? Select(string username, string password_hash)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT user_id, username, coins, elo, wins, losses FROM users WHERE username = @username && password = @password";

            RepositoryUtilities.AddParameter(command, "username", DbType.String, username);
            RepositoryUtilities.AddParameter(command, "password", DbType.String, password_hash);

            IDataReader reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            return new User()
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Coins = reader.GetInt32(2),
                Elo = reader.GetInt32(3),
                Wins = reader.GetInt32(4),
                Losses = reader.GetInt32(5),
            };
        }

        public static IEnumerable<User> Select()
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT user_id, username, coins, elo, wins, losses FROM users";

            List<User> users = new();
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User()
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Coins = reader.GetInt32(2),
                    Elo = reader.GetInt32(3),
                    Wins = reader.GetInt32(4),
                    Losses = reader.GetInt32(5),
                });
            }

            return users;
        }
    }
}
