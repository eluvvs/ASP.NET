using System.Text.Json;
using ASP.NET_MWC.Models;

namespace ASP.NET_MWC.Services
{
    public class UserStore
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<User> _users = new();

        public UserStore(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "users.json");
            Load();
        }

        // --- public API ---

        public bool Exists(string email)
            => _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public bool Register(string email, string passwordHash)
        {
            lock (_lock)
            {
                if (Exists(email)) return false;

                _users.Add(new User { Email = email, PasswordHash = passwordHash });
                Save();
                return true;
            }
        }

        public bool Validate(string email, string passwordHash)
            => _users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                && u.PasswordHash == passwordHash);

        // --- persistence ---

        private void Load()
        {
            if (!File.Exists(_filePath)) return;

            try
            {
                var json = File.ReadAllText(_filePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new();
            }
            catch { _users = new(); }
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
