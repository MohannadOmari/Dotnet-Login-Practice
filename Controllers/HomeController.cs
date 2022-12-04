using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using UmniahAssignment.Models;
using UmniahAssignment.Repository.Interface;

namespace UmniahAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepo;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("Id");
            return View();
        }

        #region Login
        [HttpGet("/Login")]
        public IActionResult Login()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("Id");
            return View();
        }

        [HttpPost("/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Users user)
        {
            try
            {
                Users? loggedIn = await _userRepo.GetUser(user);
                if (loggedIn != null)
                {
                    await HttpContext.Session.LoadAsync();
                    HttpContext.Session.SetInt32("Id", loggedIn.Id);
                    await HttpContext.Session.CommitAsync();
                    TempData["success"] = "Logged in succesfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Login information incorrect";
                    return View();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Register
        [HttpGet("/Register")]
        public IActionResult Register()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("Id");
            return View();
        }

        [HttpPost("/Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Users user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userRepo.AddUser(user);
                    return RedirectToAction("Login");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region List Users
        [HttpGet("/Users")]
        public async Task<IActionResult> ListUsers(string searchString)
        {
            try
            {
                ViewBag.UserId = HttpContext.Session.GetInt32("Id");
                if (!String.IsNullOrEmpty(searchString))
                {
                    var users = await _userRepo.AllUsers(null);
                    return View(users);
                }
                else
                {
                    var users = await _userRepo.AllUsers(searchString);
                    return View(users);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Edit & Details
        [HttpGet]
        [SecurityCheck]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                ViewBag.UserId = HttpContext.Session.GetInt32("Id");
                var user = await _userRepo.GetUserById(id);
                return View(user);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [SecurityCheck]
        public IActionResult Edit()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("Id");
            return View();
        }

        [HttpPost]
        [SecurityCheck]
        public async Task<IActionResult> Edit(Users user)
        {
            int? id = HttpContext.Session.GetInt32("Id");
            await _userRepo.UpdateUser(user, id);
            TempData["success"] = "Profile updated successfully";
            return RedirectToAction("ListUsers");
        }
        #endregion

        [LogoutCheck]
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

        #region Authorization method
        /*
        Security check for authorization
        if user not logged in route them to logging in page
        if user attempts to access another user's details route them to home page
        */
        [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]

        public class SecurityCheckAttribute : ActionFilterAttribute, IActionFilter
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var controller = context.Controller as HomeController;
                var httpContext = controller.HttpContext;

                if (httpContext.Session.GetInt32("Id") == null)
                {

                    controller.TempData["error"] = "Unauthorized Access please login";
                    context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                            { "Controller", "Home" },
                            { "Action", "Login" },
                            });
                    base.OnActionExecuting(context);
                }
                var request = controller.Request.RouteValues.Values.ToArray();
                if (httpContext.Session.GetInt32("Id").ToString() != request[request.Length - 1].ToString())
                {
                    controller.TempData["error"] = "UnAutorized Access";
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "Controller", "Home" },
                            { "Action", "Index" },
                        });
                    base.OnActionExecuting(context);
                }

                base.OnActionExecuting(context);
            }
        }
        #endregion

        #region Logout check
        [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]

        public class LogoutCheckAttribute : ActionFilterAttribute, IActionFilter
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var controller = context.Controller as HomeController;
                var httpContext = controller.HttpContext;

                if (httpContext.Session.GetInt32("Id") == null)
                {

                    controller.TempData["error"] = "Unauthorized Access please login";
                    context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                            { "Controller", "Home" },
                            { "Action", "Login" },
                            });
                    base.OnActionExecuting(context);
                }
                base.OnActionExecuting(context);
            }
        }
        #endregion
    }
}