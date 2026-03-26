using ASP.NET_MWC.Models;

namespace ASP.NET_MWC.Services
{
    public class UserStore
    {
        private readonly AppDbContext _context;

        public UserStore(AppDbContext context)
        {
            _context = context;
        }

        // --- public API ---

        public bool Exists(string username)
            => _context.Users.Any(u => u.Username.ToLower() == username.ToLower());

        public bool Register(string username, string passwordHash)
        {
            if (Exists(username)) return false;

            _context.Users.Add(new User { Username = username, PasswordHash = passwordHash });
            _context.SaveChanges();
            return true;
        }

        public bool Validate(string username, string passwordHash)
            => _context.Users.Any(u =>
                u.Username.ToLower() == username.ToLower()
                && u.PasswordHash == passwordHash);
    }
}
