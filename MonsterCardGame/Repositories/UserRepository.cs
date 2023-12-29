using MonsterCardGame.Models.DB;
using MonsterCardGame.Utilities;
using Npgsql;
using System.Data;

namespace MonsterCardGame.Repositories
{
    internal class UserRepository
    {
        public User Create(string username, string password, int coins, int elo)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "INSERT INTO users (username, password, coins, elo) VALUES (@username, @password, @coins, @elo) RETURNING user_id;";

            RepositoryUtilities.AddParameter(command, "username", DbType.String, username);
            RepositoryUtilities.AddParameter(command, "password", DbType.String, password);
            RepositoryUtilities.AddParameter(command, "coins", DbType.Int32, coins);
            RepositoryUtilities.AddParameter(command, "elo", DbType.Int32, elo);

            int userId = Convert.ToInt32(command.ExecuteScalar());
            User? user = SelectById(userId);

            if (user == null)
                throw new Exception($"[{nameof(UserRepository)}] Couldn't fetch inserted User");

            return user;
        }

        public User? SelectById(int id)
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

        public User? SelectByCredentials(string username, string password_hash)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT user_id, username, coins, elo, wins, losses FROM users WHERE username = @username AND password = @password";

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

        public IEnumerable<User> SelectAll()
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

        public User? SelectByUsername(string username)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT user_id, username, coins, elo, wins, losses FROM users WHERE username = @username";

            RepositoryUtilities.AddParameter(command, "username", DbType.String, username);

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

        public bool UpdateUsername(int userId, string newUsername)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "UPDATE users SET username = @username WHERE user_id = @user_id";
            
            RepositoryUtilities.AddParameter(command, "username", DbType.String, newUsername);
            RepositoryUtilities.AddParameter(command, "user_id", DbType.Int32, userId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
