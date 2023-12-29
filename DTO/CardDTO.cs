namespace DTO
{
    public class CardDTO
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public string ElementType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? DeckPos { get; set; } = null;
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; } = -1;
    }
}
