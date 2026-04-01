using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MWC.Models
{
    /// <summary>
    /// A private note visible only to the user who created it, shown on their profile page.
    /// </summary>
    public class PrivateNote
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
