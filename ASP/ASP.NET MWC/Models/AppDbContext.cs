using Microsoft.EntityFrameworkCore;

namespace ASP.NET_MWC.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<PrivateNote> PrivateNotes { get; set; }
    }
}
