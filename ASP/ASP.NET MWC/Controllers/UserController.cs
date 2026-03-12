using ASP.NET_MWC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserStore _store;

        public UserController(UserStore store) => _store = store;

        // GET: /User/Register
        public IActionResult Register() => View();

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(string email, string heslo)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Chyba = "Vyplňte všechna pole.";
                return View();
            }

            if (!_store.Register(email, heslo))
            {
                ViewBag.Chyba = "Účet s tímto e-mailem již existuje.";
                return View();
            }

            TempData["Zprava"] = "Registrace proběhla úspěšně! Nyní se přihlaste.";
            return RedirectToAction("Login");
        }

        // GET: /User/Login
        public IActionResult Login() => View();

        // POST: /User/Login
        [HttpPost]
        public IActionResult Login(string email, string heslo)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Chyba = "Vyplňte všechna pole.";
                return View();
            }

            // heslo arrives already SHA-256 hashed from the client
            if (!_store.Validate(email, heslo))
            {
                ViewBag.Chyba = "Nesprávný e-mail nebo heslo.";
                return View();
            }

            HttpContext.Session.SetString("Prihlasen", "true");
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserHesloHash", heslo);
            return RedirectToAction("Profil");
        }

        // GET: /User/Profil
        public IActionResult Profil()
        {
            if (HttpContext.Session.GetString("Prihlasen") != "true")
                return RedirectToAction("Login");

            var email = HttpContext.Session.GetString("UserEmail") ?? "";
            ViewBag.MaskedEmail = MaskEmail(email);
            return View();
        }

        private static string MaskEmail(string email)
        {
            int at = email.IndexOf('@');
            if (at < 1) return "***";

            string name = email[..at];
            string domain = email[(at + 1)..];

            // Mask name part: keep first and last char
            name = name.Length <= 2
                ? new string('*', name.Length)
                : $"{name[0]}***{name[^1]}";

            // Mask domain part (before last dot)
            int dot = domain.LastIndexOf('.');
            if (dot > 0)
            {
                string d = domain[..dot];
                string ext = domain[dot..];
                d = d.Length <= 2 ? new string('*', d.Length) : $"{d[0]}***{d[^1]}";
                domain = d + ext;
            }

            return $"{name}@{domain}";
        }

        // GET: /User/Odhlasit
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
