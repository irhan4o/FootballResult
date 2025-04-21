using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResult.Models.Data
{
    [Table("Team", Schema = "blg")]
    public class Team
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? NameFirstTeam { get; set; }
        [Required]
        [MaxLength(100)]
        public string? NameSecoundTeam { get; set; }
        public string? PictureFirst { get; set; }
        public string? PictureSecound { get; set; }
        public string? Description { get; set; }
        public ICollection<Stats_Teams> Stats_Teams { get; set; } = new List<Stats_Teams>();
    }
}
