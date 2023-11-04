using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models.PJWT
{
    internal class TokenContent
    {
        public int UserId { get; set; } = -1;
        public string Username { get; set; } = string.Empty;
    }
}
