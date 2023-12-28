using MonsterCardGame.Enums;
using MonsterCardGame.Models.DB;

namespace MonsterCardGame.Factories
{
    internal static class CardFactory
    {
        public static Card Random()
        {
            Random random = new();
            Card card;
            CardType[] cardTypes = Enum.GetValues<CardType>()
                .Where(type => type != CardType.Undefined)
                .ToArray();

            CardType cardType = cardTypes[random.Next(cardTypes.Count())];

            switch (cardType)
            {
                case CardType.Monster:
                    card = RandomMonster();
                    break;
                case CardType.Spell:
                    card = RandomSpell();
                    break;
                default:
                    throw new Exception($"[{nameof(CardFactory)}] Got not known card type");
            }

            return card;
        }

        public static Card RandomMonster()
        {
            Random random = new();
            MonsterCatalog[] monsters = Enum.GetValues<MonsterCatalog>()
                .Where(catalog => catalog != MonsterCatalog.Undefined)
                .ToArray();
            ElementType[] elementTypes = Enum.GetValues<ElementType>()
                .Where(type => type != ElementType.Undefined)
                .ToArray();

            MonsterCatalog monster = monsters[random.Next(monsters.Length)];
            return new Card()
            {
                Name = MonsterCatalogConverter.ToString(monster),
                Damage = random.Next(Program.MIN_DAMAGE, Program.MAX_DAMAGE + 1),
                ElementType = elementTypes[random.Next(elementTypes.Length)],
                Type = CardType.Monster,
                Description = MonsterCatalogConverter.ToDescription(monster),
            };
        }

        public static Card RandomSpell()
        {
            Random random = new();
            ElementType[] elementTypes = Enum.GetValues<ElementType>()
                .Where(type => type != ElementType.Undefined)
                .ToArray();

            return new Card()
            {
                Name = "Spell",
                Damage = random.Next(Program.MIN_DAMAGE, Program.MAX_DAMAGE + 1),
                ElementType = elementTypes[random.Next(elementTypes.Length)],
                Type = CardType.Spell,
                Description = "A powerful spell.",
            };
        }
    }
}
