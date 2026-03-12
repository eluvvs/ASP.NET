using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MWC.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
