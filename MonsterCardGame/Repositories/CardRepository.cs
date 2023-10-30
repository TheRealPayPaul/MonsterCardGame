﻿using MonsterCardGame.Enums;
using MonsterCardGame.Models;
using MonsterCardGame.Uitilities;
using Npgsql;
using System.Data;

namespace MonsterCardGame.Repositories
{
    internal enum SelectOwnerOptions
    {
        None = 0,
        OnlyDeck = 1,
    }

    internal static class CardRepository
    {
        // Create already configured in DB and give them an Owner
        public static bool CreateMultiple(IEnumerable<Card> cards, int ownerId)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbTransaction transaction = dbConnection.BeginTransaction();
            using IDbCommand command = dbConnection.CreateCommand();

            try
            {
                dbConnection.Open();

                command.CommandText = "INSERT INTO cards (name, damage, element_type, card_type, description, fk_owner_id)" +
                                      "VALUES (@name, @damage, @element_type, @card_type, @description, @fk_owner_id); SELECT SCOPE_IDENTITY();";

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

        // Select all cards of User
        // OnlyDeck Option: Select all cards if User in the current deck
        public static IEnumerable<Card> SelectAllOfUser(int ownerId, SelectOwnerOptions options = SelectOwnerOptions.None)
        {
            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            if ((options & SelectOwnerOptions.OnlyDeck) == SelectOwnerOptions.OnlyDeck)
            {
                command.CommandText = "SELECT card_id, name, damage, element_type, card_type, deck_pos, description, fk_owner_id FROM cards" +
                                      "WHERE fk_owner_id = @fk_owner_id";
            }
            else
            {
                command.CommandText = "SELECT card_id, name, damage, element_type, card_type, deck_pos, description, fk_owner_id FROM cards" +
                                      "WHERE fk_owner_id = @fk_owner_id AND deck_pos NOT NULL";
            }

            RepositoryUtilities.AddParameter(command, "fk_owner_id", DbType.Int32, ownerId);

            List<Card> cards = new();
            IDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                cards.Add(new Card()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Damage = reader.GetInt32(2),
                    ElementType = ElementTypeConverter.ToEnum(reader.GetString(3)),
                    Type = CardTypeConverter.ToEnum(reader.GetString(4)),
                    DeckPos = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    Description = reader.GetString(6),
                    OwnerId = reader.GetInt32(7),
                });
            }

            return cards;
        }

        // Update Deck pos
        public static bool UpdateDeck(Card[] newDeck)
        {
            if (newDeck.Count() < 1)
                throw new Exception($"[{nameof(CardRepository)}] Can't update deck. No cards given.");

            if (newDeck.DistinctBy(card => card.OwnerId).Count() > 1)
                throw new Exception($"[{nameof(CardRepository)}] Can't update deck. Given cards don't have the same owner.");

            IEnumerable<Card> currentDeck = SelectAllOfUser(newDeck[0].OwnerId, SelectOwnerOptions.OnlyDeck);

            using IDbConnection dbConnection = new NpgsqlConnection(Program.CONNECTION_STRING);
            using IDbTransaction transaction = dbConnection.BeginTransaction();
            using IDbCommand command = dbConnection.CreateCommand();
            dbConnection.Open();

            try
            {
                // Set deck positions to NULL for old deck cards
                command.CommandText = "UPDATE cards SET deck_pos = NULL WHERE card_id = @card_id";
                foreach (Card card in currentDeck)
                {
                    RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, card.Id);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                // Set deck positions for new deck cards
                command.CommandText = "UPDATE cards SET deck_pos = @deck_pos WHERE card_id = @card_id";
                for (int i = 0; i < newDeck.Count(); i++)
                {
                    Card card = newDeck[i];

                    RepositoryUtilities.AddParameter(command, "deck_pos", DbType.Int32, i);
                    RepositoryUtilities.AddParameter(command, "card_id", DbType.Int32, card.Id);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    card.DeckPos = i;
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{nameof(CardRepository)}] failed to update deck: {ex.Message}");
                transaction.Rollback();
                return false;
            }
        }
    }
}
