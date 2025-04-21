using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResult.Models.Data
{
    [Table("Stats_Teams", Schema = "blg")]
    public class Stats_Teams
    {       
        public int Id { get; set; }
        [ForeignKey(nameof(Team))]
        [Required]
        public int TeamFId { get; set; }
        public Team? Team { get; set; }

        [ForeignKey(nameof(Stats))]
        [Required]
        public int StatsFId {get; set;}
        public Stats? Stats { get; set; }
       
    }
}
