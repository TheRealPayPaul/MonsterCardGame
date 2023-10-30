using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Enums
{
    public enum CardType
    {
        Undefined,
        Monster,
        Spell,
    }

    public static class CardTypeConverter
    {
        public static string ToString(CardType type)
        {
            switch (type)
            {
                case CardType.Monster:
                    return "Monster";
                case CardType.Spell:
                    return "Spell";
                default:
                    return "Undefined";
            }
        }

        public static CardType ToEnum(string stringType)
        {
            switch (stringType)
            {
                case "Monster":
                    return CardType.Monster;
                case "Spell":
                    return CardType.Spell;
                default: 
                    return CardType.Undefined;
            }
        }
    }
}
