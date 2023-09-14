using MonsterCardGame.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{
    public class CardPackage
    {
        public const int CARD_COUNT = 5;
        public const int COIN_COST = 5;
        public Card[] Cards { get; private set; }

        public CardPackage(IEnumerable<Card> cards)
        {
            if (cards.Count() != CARD_COUNT)
                throw new Exception($"[{nameof(CardPackage)}] Invalid card count. {CARD_COUNT} needed {cards.Count()} given.");

            Cards = cards.ToArray();
        }
    }
}
