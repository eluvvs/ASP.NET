using System.Diagnostics;
using ASP.NET_MWC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MWC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db) => _db = db;

        private void LoadPageData(string pageKey)
        {
            ViewBag.PageKey = pageKey;
            ViewBag.Comments = _db.Comments
                .Where(c => c.PageKey == pageKey)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            ViewBag.Notes = _db.Notes
                .Where(n => n.PageKey == pageKey)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("Prihlasen") == "true";
            ViewBag.CurrentUser = HttpContext.Session.GetString("UserName") ?? "";
        }

        public IActionResult Index()
        {
            LoadPageData("Index");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            LoadPageData("Pricing");
            return View();
        }

        public IActionResult Bomb()
        {
            LoadPageData("Bomb");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
