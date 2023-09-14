using MonsterCardGame.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{
    public class CardStack
    {
        public List<Card> Cards { get; private set; }
        public CardStack(IEnumerable<Card> cards)
        {
            Cards = cards.ToList();
        }
    }
}
