using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Utilities;
using Npgsql;
using System.Data;

namespace MonsterCardGame.Repositories
{
    internal class CompositeRepository
    {
        private readonly TradeRepository _tradeRepository;
        private readonly UserRepository _userRepository;

        public CompositeRepository()
        {
            _tradeRepository = new TradeRepository();
            _userRepository = new UserRepository();
        }

        public CompositeRepository(TradeRepository tradeRepository, UserRepository userRepository)
        {
            _tradeRepository = tradeRepository;
            _userRepository = userRepository;
        }

        public bool TradeCards(int tradeId, Card toBeTradedCard)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            dbConnection.Open();
            using IDbTransaction transaction = dbConnection.BeginTransaction();

            Trade? trade = _tradeRepository.SelectById(tradeId);
            if (trade == null )
            {
                transaction.Rollback();
                return false;
            }

            // Switch owner first card
            using IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = "UPDATE cards SET fk_owner_id = @fk_owner_id WHERE card_id = @card_id";

            RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, trade.OfferedCard.OwnerId);
            RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, toBeTradedCard.Id);

            if (command.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }

            command.Parameters.Clear();

            // Switch owner second card
            RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, toBeTradedCard.OwnerId);
            RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, trade.OfferedCard.Id);

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
        public bool BuyCards(IEnumerable<Card> cards, int cost, int ownerId)
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

        public bool EndOfBattleNonDraw(int loserId, int winnerId, List<Card> winnerCards)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            dbConnection.Open();
            using IDbTransaction transaction = dbConnection.BeginTransaction();
            using IDbCommand command = dbConnection.CreateCommand();

            User? loserUser = _userRepository.SelectById(loserId);
            if (loserUser == null )
            {
                transaction.Rollback();
                return false;
            }
            
            User? winnerUser = _userRepository.SelectById(winnerId);
            if (winnerUser == null )
            {
                transaction.Rollback();
                return false;
            }
            
            command.CommandText = "UPDATE users SET losses = @losses, elo = @elo WHERE user_id = @user_id";
            
            RepositoryUtilities.AddParameter(command, "losses", DbType.Int32, loserUser.Losses + 1);
            RepositoryUtilities.AddParameter(command, "elo", DbType.Int32, loserUser.Elo - 5);
            RepositoryUtilities.AddParameter(command, "user_id", DbType.Int32, loserUser.Id);

            if (command.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }
            
            command.Parameters.Clear();
            
            command.CommandText = "UPDATE users SET wins = @wins, elo = @elo WHERE user_id = @user_id";
            
            RepositoryUtilities.AddParameter(command, "wins", DbType.Int32, winnerUser.Wins + 1);
            RepositoryUtilities.AddParameter(command, "elo", DbType.Int32, winnerUser.Elo + 3);
            RepositoryUtilities.AddParameter(command, "user_id", DbType.Int32, winnerUser.Id);
            
            if (command.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }
            
            command.Parameters.Clear();

            foreach (Card card in winnerCards)
            {
                if (card.OwnerId == winnerUser.Id)
                    continue;
                
                command.CommandText = "UPDATE cards SET fk_owner_id = @fk_owner_id, deck_pos = NULL WHERE card_id = @card_id";
                
                RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, winnerUser.Id);
                RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, card.Id);
            
                if (command.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            
                command.Parameters.Clear();
            }

            transaction.Commit();
            return true;
        }
        
        public bool EndOfBattleDraw(int player1Id, List<Card> player1Cards, int player2Id, List<Card> player2Cards)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            dbConnection.Open();
            using IDbTransaction transaction = dbConnection.BeginTransaction();
            using IDbCommand command = dbConnection.CreateCommand();
            
            command.CommandText = "UPDATE cards SET fk_owner_id = @fk_owner_id, deck_pos = NULL WHERE card_id = @card_id";
            
            foreach (Card card in player1Cards)
            {
                RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, player1Id);
                RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, card.Id);
            
                if (command.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            
                command.Parameters.Clear();
            }
            
            foreach (Card card in player2Cards)
            {
                RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, player2Id);
                RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, card.Id);
            
                if (command.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            
                command.Parameters.Clear();
            }
            
            transaction.Commit();
            return true;
        }
    }
}
