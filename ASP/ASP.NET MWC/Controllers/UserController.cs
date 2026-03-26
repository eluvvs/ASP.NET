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

            // heslo arrives already SHA-256 hashed from the client
            if (!_store.Validate(username, heslo))
            {
                ViewBag.Chyba = "Nesprávné uživatelské jméno nebo heslo.";
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
