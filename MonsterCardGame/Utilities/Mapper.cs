using DTO;
using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;

namespace MonsterCardGame.Utilities
{
    public class Mapper
    {
        public virtual CardDTO ToDTO(Card card)
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

        public virtual CardDTO[] ToDTO(Card[] cards)
        {
            CardDTO[] cardsDTO = new CardDTO[cards.Length];
            for (int i = 0; i < cards.Length; i++)
                cardsDTO[i] = ToDTO(cards[i]);
            
            return cardsDTO;
        }

        public virtual TradeDTO ToDTO(Trade trade)
        {
            return new TradeDTO()
            {
                Id = trade.Id,
                CardToTrade = ToDTO(trade.OfferedCard),
                WantedCardType = CardTypeConverter.ToString(trade.OfferedCard.Type),
                WantedMinDamage = trade.WantedMinDamage,
            };
        }

        public virtual TradeDTO[] ToDTO(Trade[] trades)
        {
            TradeDTO[] tradesDTO = new TradeDTO[trades.Length];
            for (int i = 0; i < trades.Length; i++)
                tradesDTO[i] = ToDTO(trades[i]);

            return tradesDTO;
        }

        public virtual TokenContent ToTokenContent(User user)
        {
            return new TokenContent()
            {
                UserId = user.Id,
            };
        }

        public virtual UserStatsDTO ToStats(User user)
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
