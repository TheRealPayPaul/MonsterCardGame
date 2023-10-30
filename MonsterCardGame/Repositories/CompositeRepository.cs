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
    }
}
