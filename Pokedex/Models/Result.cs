using SQLite;

namespace Pokedex.Models
{
    [Table("Results")]
    public class Result
    {
        [NotNull]
        [Column("id")]
        public int ID { get; set; }

        [NotNull]
        [Column("identifier")]
        public string Identifier { get; set; }

        [NotNull]
        [Column("typename")]
        public string TypeName { get; set; }
    }
}
