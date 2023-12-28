using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Enums
{
    public enum MonsterCatalog
    {
        Undefined,
        Goblin,
        Troll,
        Elf,
        Knight,
        Dragon,
        Ork,
        Kraken,
        Wizzard,
    }

    public static class MonsterCatalogConverter
    {
        public static MonsterCatalog ToEnum(string name)
        {
            switch (name)
            {
                case "Goblin":
                    return MonsterCatalog.Goblin;
                case "Troll":
                    return MonsterCatalog.Troll;
                case "Elf":
                    return MonsterCatalog.Elf;
                case "Knight":
                    return MonsterCatalog.Knight;
                case "Dragon":
                    return MonsterCatalog.Dragon;
                case "Ork":
                    return MonsterCatalog.Ork;
                case "Kraken":
                    return MonsterCatalog.Kraken;
                case "Wizzard":
                    return MonsterCatalog.Wizzard;
                default:
                    return MonsterCatalog.Undefined;
            }
        }
        
        public static string ToString(MonsterCatalog monster)
        {
            switch (monster)
            {
                case MonsterCatalog.Goblin:
                    return "Goblin";
                case MonsterCatalog.Troll:
                    return "Troll";
                case MonsterCatalog.Elf:
                    return "Elf";
                case MonsterCatalog.Knight:
                    return "Knight";
                case MonsterCatalog.Dragon:
                    return "Dragon";
                case MonsterCatalog.Ork:
                    return "Ork";
                case MonsterCatalog.Kraken:
                    return "Kraken";
                case MonsterCatalog.Wizzard:
                    return "Wizzard";
                default:
                    return "Undefined";
            }
        }

        public static string ToDescription(MonsterCatalog monster)
        {
            switch (monster)
            {
                case MonsterCatalog.Goblin:
                    return "Mischievous and small in stature, Goblins are known for their cunning tricks and love of shiny objects. They often inhabit dark forests and hidden caves.";
                case MonsterCatalog.Troll:
                    return "Towering and fearsome, Trolls possess incredible strength and regenerative abilities. Their rocky skin and brute force make them formidable foes.";
                case MonsterCatalog.Elf:
                    return "Graceful and ethereal, Elves are mystical beings with a deep connection to nature. They are skilled archers and often reside in enchanting woodlands.";
                case MonsterCatalog.Knight:
                    return "Noble and valiant, Knights are chivalrous warriors who don formidable armor and wield mighty swords. They protect the realm and uphold honor.";
                case MonsterCatalog.Dragon:
                    return "Majestic and powerful, Dragons are ancient creatures with the ability to breathe fire. They hoard treasure and are often the stuff of legends.";
                case MonsterCatalog.Ork:
                    return "Brutish and relentless, Orcs are fierce warriors who thrive in harsh environments. They are known for their aggressive nature and tribal societies.";
                case MonsterCatalog.Kraken:
                    return "Enormous and mysterious, Krakens dwell in the depths of the ocean. These tentacled sea monsters are said to bring terror to sailors with their immense power.";
                case MonsterCatalog.Wizzard:
                    return "A wise wizard in mystical robes and a crackling staff, guiding through arcane realms with ancient knowledge and potent spells.";
                default:
                    return "Undefined";
            }
        }
    }
}
