using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class CreateTradeDTO
    {
        public int Id { get; set; }
        public int CardToTradeId { get; set; }
        public string WantedCardType { get; set; } = string.Empty;
        public int WantedMinDamage { get; set; }
    }
}
