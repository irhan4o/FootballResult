using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FootballResult.Models.Data
{
    public class User:IdentityUser
    {
        [Key]
        public int? Pk { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Range(18, int.MaxValue, ErrorMessage = "You must be at least 18 years old to be permitted to book!")]
        public uint? Age { get; set; } = null;
        public string? ProfilePicture { get; set; }
    }
}
