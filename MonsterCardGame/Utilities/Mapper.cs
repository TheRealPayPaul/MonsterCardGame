using DTO;
using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Utilities
{
    internal static class Mapper
    {
        public static CardDTO ToDTO(Card card)
        {
            return new CardDTO()
            {
                Id = card.Id,
                Name = card.Name,
                Damage = card.Damage,
                ElementType = ElementTypeConverter.ToString(card.ElementType),
                Type = CardTypeConverter.ToString(card.Type),
                DeckPos = card.DeckPos,
                Description = card.Description,
                OwnerId = card.OwnerId,
            };
        }

        public static CardDTO[] ToDTO(Card[] cards)
        {
            CardDTO[] cardsDTO = new CardDTO[cards.Length];
            for (int i = 0; i < cards.Length; i++)
                cardsDTO[i] = ToDTO(cards[i]);
            
            return cardsDTO;
        }

        public static TokenContent ToTokenContent(User user)
        {
            return new TokenContent()
            {
                UserId = user.Id,
                Username = user.Username,
            };
        }

        public static UserStatsDTO ToStats(User user)
        {
            return new UserStatsDTO()
            {
                Username = user.Username,
                Elo = user.Elo,
                Wins = user.Wins,
                Losses = user.Losses,
            };
        }
    }
}
