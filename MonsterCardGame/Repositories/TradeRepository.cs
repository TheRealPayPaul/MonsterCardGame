using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Utilities;
using Npgsql;
using System.Data;


namespace MonsterCardGame.Repositories
{
    internal class TradeRepository
    {
        public IEnumerable<Trade> SelectAll()
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT " +
                                  "trade_id, wanted_card_type, wanted_min_damage, card_id, name, damage, element_type, card_type, deck_pos, description, user_id, username" +
                                  " FROM trade_overviews";

            List<Trade> trades = new();
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Card offeredCard = new()
                {
                    Id = reader.GetInt32(3),
                    Name = reader.GetString(4),
                    Damage = reader.GetInt32(5),
                    ElementType = ElementTypeConverter.ToEnum(reader.GetString(6)),
                    Type = CardTypeConverter.ToEnum(reader.GetString(7)),
                    DeckPos = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                    Description = reader.GetString(9),
                    OwnerId = reader.GetInt32(10),
                };

                trades.Add(new Trade(offeredCard)
                {
                    Id = reader.GetInt32(0),
                    WantedCardType = CardTypeConverter.ToEnum(reader.GetString(1)),
                    WantedMinDamage = reader.GetInt32(2),
                    TraderName = reader.GetString(11),
                });
            }

            return trades;
        }

        public Trade? SelectById(int tradeId)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT " +
                                  "trade_id, wanted_card_type, wanted_min_damage, card_id, name, damage, element_type, card_type, description, user_id, username" +
                                  " FROM trade_overviews WHERE trade_id = @trade_id";

            RepositoryUtilities.AddParameter(command, "trade_id", DbType.Int32, tradeId);

            IDataReader reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            Card offeredCard = new()
            {
                Id = reader.GetInt32(3),
                Name = reader.GetString(4),
                Damage = reader.GetInt32(5),
                ElementType = ElementTypeConverter.ToEnum(reader.GetString(6)),
                Type = CardTypeConverter.ToEnum(reader.GetString(7)),
                DeckPos = null,
                Description = reader.GetString(8),
                OwnerId = reader.GetInt32(9),
            };

            return new Trade(offeredCard)
            {
                Id = reader.GetInt32(0),
                WantedCardType = CardTypeConverter.ToEnum(reader.GetString(1)),
                WantedMinDamage = reader.GetInt32(2),
                TraderName = reader.GetString(10),
            };
        }

        public bool Create(int cardIdToTrade, CardType wantedCardType, int wantedMinDamage)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "INSERT INTO trades (fk_offered_card_id, wanted_card_type, wanted_min_damage) " +
                                  "VALUES (@fk_offered_card_id, @wanted_card_type, @wanted_min_damage)";

            RepositoryUtilities.AddParameter(command, "fk_offered_card_id", DbType.Int32, cardIdToTrade);
            RepositoryUtilities.AddParameter(command, "wanted_card_type", DbType.String, CardTypeConverter.ToString(wantedCardType));
            RepositoryUtilities.AddParameter(command, "wanted_min_damage", DbType.Int32, wantedMinDamage);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int tradeId)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "DELETE FROM trades WHERE trade_id = @trade_id";

            RepositoryUtilities.AddParameter(command, "trade_id", DbType.Int32, tradeId);

            return command.ExecuteNonQuery() > 0;
        }

        public bool IsCardInTrade(int cardId)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            command.CommandText = "SELECT trade_id FROM trades WHERE fk_offered_card_id = @fk_offered_card_id";

            RepositoryUtilities.AddParameter(command, "fk_offered_card_id", DbType.Int32, cardId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
