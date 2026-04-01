using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MWC.Models
{
    /// <summary>
    /// Public note attached to an article page, visible to all logged-in users.
    /// </summary>
    public class Note
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Which page/article the note belongs to, e.g. "Index", "Pricing", "Bomb".</summary>
        public string PageKey { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
