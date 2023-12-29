using MonsterCardGame.Enums;

namespace MonsterCardGame.Models.DB
{
    internal class Trade
    {
        public int Id { get; set; }
        public string TraderName { get; set; } = string.Empty;
        public CardType WantedCardType { get; set; }
        public int WantedMinDamage { get; set; }
        public Card OfferedCard { get; set; }

        public Trade(Card offeredCard)
        {
            OfferedCard = offeredCard;
        }
    }
}
