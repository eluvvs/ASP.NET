using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MWC.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Which page/article the comment belongs to, e.g. "Index", "Pricing", "Bomb".</summary>
        public string PageKey { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
