using ASP.NET_MWC.Models;
using ASP.NET_MWC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserStore _store;
        private readonly AppDbContext _db;

        public UserController(UserStore store, AppDbContext db)
        {
            _store = store;
            _db = db;
        }

        // GET: /User/Register
        public IActionResult Register() => View();

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(string username, string heslo)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Chyba = "Vyplňte všechna pole.";
                return View();
            }

            if (!_store.Register(username, heslo))
            {
                ViewBag.Chyba = "Účet s tímto jménem již existuje.";
                return View();
            }

            TempData["Zprava"] = "Registrace proběhla úspěšně! Nyní se přihlaste.";
            return RedirectToAction("Login");
        }

        // GET: /User/Login
        public IActionResult Login() => View();

        // POST: /User/Login
        [HttpPost]
        public IActionResult Login(string username, string heslo)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Chyba = "Vyplňte všechna pole.";
                return View();
            }

            // Check existence
            if (!_store.Exists(username))
            {
                ViewBag.Chyba = "Tento uživatel neexistuje.";
                return View();
            }

            // Check password
            if (!_store.Validate(username, heslo))
            {
                ViewBag.Chyba = "Nesprávné heslo.";
                return View();
            }

            HttpContext.Session.SetString("Prihlasen", "true");
            HttpContext.Session.SetString("UserName", username);
            HttpContext.Session.SetString("UserHesloHash", heslo);
            return RedirectToAction("Profil");
        }

        // GET: /User/Profil
        public IActionResult Profil()
        {
            if (HttpContext.Session.GetString("Prihlasen") != "true")
                return RedirectToAction("Login");

            var username = HttpContext.Session.GetString("UserName") ?? "";
            ViewBag.UserName = username;
            ViewBag.PrivateNotes = _db.PrivateNotes
                .Where(n => n.Username == username)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return View();
        }

        // GET: /User/Odhlasit
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
