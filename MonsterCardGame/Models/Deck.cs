using MonsterCardGame.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{
    public class Deck
    {
        public const int DECK_SIZE = 4;
        
        public List<Card> Cards;

        public Deck(IEnumerable<Card> cards)
        {
            if (cards.Count() != DECK_SIZE)
                throw new ArgumentException($"[{nameof(Deck)}] given cards {cards.Count()}, needed {DECK_SIZE}.");

            Cards = cards.ToList();
        }
    }
}
