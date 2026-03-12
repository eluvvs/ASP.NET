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

        public bool Exists(string email)
            => _context.Users.Any(u => u.Email.ToLower() == email.ToLower());

        public bool Register(string email, string passwordHash)
        {
            if (Exists(email)) return false;

            _context.Users.Add(new User { Email = email, PasswordHash = passwordHash });
            _context.SaveChanges();
            return true;
        }

        public bool Validate(string email, string passwordHash)
            => _context.Users.Any(u =>
                u.Email.ToLower() == email.ToLower()
                && u.PasswordHash == passwordHash);
    }
}
