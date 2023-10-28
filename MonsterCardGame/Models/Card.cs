using MonsterCardGame.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{

    public class Card
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public ElementType ElementType { get; set; }
        public CardType Type { get; set; }
        public bool IsInTrade { get; set; }
        public int? DeckPos { get; set; }
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; } = -1;
    }
}
