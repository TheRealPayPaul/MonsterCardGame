using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TradeDTO
    {
        public int Id { get; set; }
        public CardDTO CardToTrade { get; set; } = new();
        public string WantedCardType { get; set; } = string.Empty;
        public int WantedMinDamage { get; set; }
    }
}
