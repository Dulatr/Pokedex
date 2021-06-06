using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace Pokedex.Models
{
    [Table("pokemon")]
    public class Pokemon
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int ID { get; set; }

        [NotNull]
        [Column("identifier")]
        public string Identifier { get; set; }

        [NotNull]
        [Column("species_id")]
        public int Species_id { get; set; }

        [NotNull]
        [Column("height")]
        public int Height { get; set; }

        [NotNull]
        [Column("weight")]
        public int Weight { get; set; }

        [NotNull]
        [Column("base_experience")]
        public int Base_Experience { get; set; }

        [Column("order")]
        public int Order { get; set; }

        [NotNull]
        [Column("is_default")]
        public bool Is_Default { get; set; }
    }
}
