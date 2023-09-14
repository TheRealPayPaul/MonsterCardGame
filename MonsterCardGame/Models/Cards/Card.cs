using MonsterCardGame.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Models.Cards
{

    public abstract class Card
    {
        public readonly string Uuid;
        public readonly string CatalogUuid;
        public string Name { get; private set; }
        public readonly float Damage;
        public ElementType ElementType { get; private set; }
        public CardType Type { get; private set; }

        protected Card(
            string uuid,
            string catalogUuid,
            string name,
            float damage,
            ElementType elementType,
            CardType type )
        {
            Uuid = uuid;
            CatalogUuid = catalogUuid;
            Name = name;
            Damage = damage;
            ElementType = elementType;
            Type = type;
        }
    }
}
