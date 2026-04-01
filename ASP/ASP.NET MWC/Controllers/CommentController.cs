using ASP.NET_MWC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _db;

        public CommentController(AppDbContext db) => _db = db;

        [HttpPost]
        public IActionResult AddComment(string pageKey, string content, string returnUrl)
        {
            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(content))
                return Redirect(returnUrl ?? "/");

            _db.Comments.Add(new Comment
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
