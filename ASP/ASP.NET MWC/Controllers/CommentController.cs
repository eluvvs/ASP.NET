using ASP.NET_MWC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CommentController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string pageKey, string content, string returnUrl, IFormFile? image)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(content))
                return Redirect(returnUrl ?? "/");

            string? imagePath = null;

            if (image != null && image.Length > 0)
            {
                // Validate file type
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                    return Redirect(returnUrl ?? "/");

                // Validate file size (max 5 MB)
                if (image.Length > 5 * 1024 * 1024)
                    return Redirect(returnUrl ?? "/");

                // Save to wwwroot/uploads/comments/
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "comments");
                Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString("N") + ext;
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = fileName;
            }

            _db.Comments.Add(new Comment
            {
                PageKey = pageKey,
                Username = username,
                Content = content.Trim(),
                ImagePath = imagePath,
                CreatedAt = DateTime.UtcNow
            });
            _db.SaveChanges();

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        public IActionResult DeleteComment(int id, string returnUrl)
        {
            var username = HttpContext.Session.GetString("UserName");
            var comment = _db.Comments.Find(id);
            if (comment != null && comment.Username == username)
            {
                _db.Comments.Remove(comment);
                _db.SaveChanges();
            }
            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        public IActionResult AddNote(string pageKey, string content, string returnUrl)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(content))
                return Redirect(returnUrl ?? "/");

            _db.Notes.Add(new Note
            {
                PageKey = pageKey,
                Username = username,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            });
            _db.SaveChanges();

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        public IActionResult DeleteNote(int id, string returnUrl)
        {
            var username = HttpContext.Session.GetString("UserName");
            var note = _db.Notes.Find(id);
            if (note != null && note.Username == username)
            {
                _db.Notes.Remove(note);
                _db.SaveChanges();
            }
            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        public IActionResult AddPrivateNote(string content)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Profil", "User");

            _db.PrivateNotes.Add(new PrivateNote
            {
                Username = username,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            });
            _db.SaveChanges();

            return RedirectToAction("Profil", "User");
        }

        [HttpPost]
        public IActionResult DeletePrivateNote(int id)
        {
            var username = HttpContext.Session.GetString("UserName");
            var note = _db.PrivateNotes.Find(id);
            if (note != null && note.Username == username)
            {
                _db.PrivateNotes.Remove(note);
                _db.SaveChanges();
            }
            return RedirectToAction("Profil", "User");
        }
    }
}
