using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Utilities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Repositories
{
    internal static class CompositeRepository
    {
        public static bool TradeCards(int tradeId, Card toBeTradedCard)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbTransaction transaction = dbConnection.BeginTransaction();
            dbConnection.Open();

            Trade? trade = TradeRepository.SelectById(tradeId);
            if (trade == null )
            {
                transaction.Rollback();
                return false;
            }

            // Switch owner first card
            using IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = "UPDATE cards SET fk_owner_id = @fk_owner_id WHERE card_id = @card_id";

            RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, trade.OfferedCard.OwnerId);
            RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, toBeTradedCard.OwnerId);

            if (command.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }

            command.Parameters.Clear();

            // Switch owner second card
            RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, trade.OfferedCard.OwnerId);
            RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, toBeTradedCard.OwnerId);

            if (command.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }

            command.Parameters.Clear();

            // Delete trade
            command.CommandText = "DELETE FROM trades WHERE trade_id = @trade_id";

            RepositoryUtilities.AddParameter(command, "trade_id", DbType.Int32, tradeId);

            if (command.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }

            transaction.Commit();
            return true;
        }

        // Create given cards in DB give them to owner and take coins
        public static bool BuyCards(IEnumerable<Card> cards, int cost, int ownerId)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            dbConnection.Open();
            using IDbTransaction transaction = dbConnection.BeginTransaction();
            using IDbCommand command = dbConnection.CreateCommand();

            try
            {
                command.CommandText = "UPDATE users SET coins = coins - @cost WHERE user_id = @user_id";
                
                RepositoryUtilities.AddParameter(command, "cost", DbType.Int32, cost);
                RepositoryUtilities.AddParameter(command, "user_id", DbType.Int32, ownerId);

                if (command.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
                command.Parameters.Clear();

                command.CommandText = "INSERT INTO cards (name, damage, element_type, card_type, description, fk_owner_id)" +
                                      "VALUES (@name, @damage, @element_type, @card_type, @description, @fk_owner_id) RETURNING card_id;";

                foreach (Card card in cards)
                {
                    RepositoryUtilities.AddParameter(command, "name", DbType.String, card.Name);
                    RepositoryUtilities.AddParameter(command, "damage", DbType.Int32, card.Damage);
                    RepositoryUtilities.AddParameter(command, "element_type", DbType.String, ElementTypeConverter.ToString(card.ElementType));
                    RepositoryUtilities.AddParameter(command, "card_type", DbType.String, CardTypeConverter.ToString(card.Type));
                    RepositoryUtilities.AddParameter(command, "description", DbType.String, card.Description);
                    RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, ownerId);

                    card.DeckPos = null;
                    card.OwnerId = ownerId;
                    card.Id = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{nameof(CardRepository)}] failed to insert all cards: {ex.Message}");
                transaction.Rollback();
                return false;
            }
        }
    }
}
