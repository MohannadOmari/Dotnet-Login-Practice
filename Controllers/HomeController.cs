using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UmniahAssignment.Models;

namespace UmniahAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Login
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }
        #endregion

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}