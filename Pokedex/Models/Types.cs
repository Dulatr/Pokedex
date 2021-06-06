using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace Pokedex.Models
{
    [Table("types")]
    public class Types
    {
        [NotNull]
        [Column("id")]
        public int ID { get; set; }

        [NotNull]
        [Column("identifier")]
        public string Identifier { get; set; }

        [NotNull]
        [Column("generation_id")]
        public int Generation_ID { get; set; }

        [Column("damage_class_id")]
        public int Damage_Class_ID { get; set; }
    }
}
