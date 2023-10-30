using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{
    internal class User
    {
        public int Id { get; set; } = -1;
        public string Username { get; set; } = string.Empty;
        public int Coins { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
