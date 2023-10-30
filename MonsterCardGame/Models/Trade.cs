using MonsterCardGame.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{
    internal class Trade
    {
        public int Id { get; set; }
        public string TraderName { get; set; } = string.Empty;
        public CardType WantedCardType { get; set; }
        public int WantedMinDamage { get; set; }
        public Card OfferedCard { get; set; }

        public Trade(Card offeredCard)
        {
            OfferedCard = offeredCard;
        }
    }
}
