using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace FamilyBudget.Next.Controllers
{
    public class UserController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // GET: /<controller>/
        [HttpPost]
        public IActionResult Index(string userName)
        {
            var identity = new GenericIdentity("Artem");
            var principal = new GenericPrincipal(identity, null);
            var task = HttpContext.Authentication.SignInAsync("FBMiddlewareInstance", principal);

            Task.WaitAll(task);

            return View();
        }

        // GET: /<controller>/
        public IActionResult Unauthorized()
        {
            return View();
        }

        // GET: /<controller>/
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
