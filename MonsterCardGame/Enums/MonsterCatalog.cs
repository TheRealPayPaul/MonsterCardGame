using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Enums
{
    public enum MonsterCatalog
    {
        Goblin,
        Troll,
        Elf,
        Knight,
        Dragon,
        Ork,
        Kraken
    }

    public static class MonsterCatalogConverter
    {
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
                default:
                    return "Undefined";
            }
        }

        public static string ToDescription(MonsterCatalog monster)
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
                default:
                    return "Undefined";
            }
        }
    }
}
