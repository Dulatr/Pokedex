using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace Pokedex.Models
{
    [Table("pokemon_types")]
    public class PokemonTypes
    {
        [NotNull]
        [Column("pokemon_id")]
        public int Pokemon_ID { get; set; }

        [NotNull]
        [Column("type_id")]
        public int Type_ID { get; set; }

        [NotNull]
        [Column("slot")]
        public int Slot { get; set; }
    }
}
