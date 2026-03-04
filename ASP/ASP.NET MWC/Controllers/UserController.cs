using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class UserController : Controller
    {
        // GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(string jmeno, string email, string heslo)
        {
            // Tady by normálně bylo uložení do databáze
            // Pro teď jen přesměrujeme na přihlášení
            TempData["Zprava"] = "Registrace proběhla úspěšně! Nyní se přihlaste.";
            return RedirectToAction("Login");
        }

        // GET: /User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        public IActionResult Login(string email, string heslo)
        {
            // Tady by normálně bylo ověření proti databázi
            // Pro teď jen uložíme do session že je přihlášen
            HttpContext.Session.SetString("Prihlasen", "true");
            HttpContext.Session.SetString("UserJmeno", "Jan Novák");
            HttpContext.Session.SetString("UserEmail", email);
            return RedirectToAction("Profil");
        }

        // GET: /User/Profil
        public IActionResult Profil()
        {
            // Kontrola přihlášení
            if (HttpContext.Session.GetString("Prihlasen") != "true")
            {
                return RedirectToAction("Login");
            }

            ViewBag.Jmeno = HttpContext.Session.GetString("UserJmeno");
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
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
