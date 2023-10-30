using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Enums
{
    public enum ElementType
    {
        Undefined,
        Fire,
        Water,
        Normal
    }

    public static class ElementTypeConverter
    {
        public static string ToString(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire:
                    return "Fire";
                case ElementType.Water:
                    return "Water";
                case ElementType.Normal:
                    return "Normal";
                default:
                    return "Undefined";
            }
        }

        public static ElementType ToEnum(string stringType)
        {
            switch (stringType)
            {
                case "Fire":
                    return ElementType.Fire;
                case "Water":
                    return ElementType.Water;
                case "Normal":
                    return ElementType.Normal;
                default:
                    return ElementType.Undefined;
            }
        }
    }
}
