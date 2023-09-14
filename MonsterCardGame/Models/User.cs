using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models
{
    public class User
    {
        public string Username { get; private set; }
        public CardStack CardStack { get; private set; }
        public int Coins { get; private set; }

        public User(string username, CardStack cardStack, int coins) 
        {
            Username = username;
            CardStack = cardStack;
            Coins = coins;
        }
    }
}
