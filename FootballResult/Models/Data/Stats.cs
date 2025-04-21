using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResult.Models.Data
{
    [Table("Stats", Schema = "blg")]
    public class Stats
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int resultFirstTeam {  get; set; }
        [Required]
        public int resultOtherTeam { get; set; }
        public uint Corner {  get; set; }
        public bool IsFinished { get; set; } = false;
        public ICollection<Stats_Teams> Stats_Teams { get; set; } = new List<Stats_Teams>();
    }
}
