using MonsterCardGame.Enums;

namespace MonsterCardGame.Models.DB
{

    public class Card
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public ElementType ElementType { get; set; }
        public CardType Type { get; set; }
        public int? DeckPos { get; set; }
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; } = -1;
    }
}
