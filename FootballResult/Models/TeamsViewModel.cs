using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FootballResult.Models
{
    public class TeamsViewModel
    {

        public int Id { get; set; }
        public string? NameFirstTeam { get; set; }
        public string? NameSecoundTeam { get; set; }
        public IFormFile? PictureFirst { get; set; }
        public IFormFile? PictureSecound { get; set; }
        public string? Description { get; set; }
    }
}
