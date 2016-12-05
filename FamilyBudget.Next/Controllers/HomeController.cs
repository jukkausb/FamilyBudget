using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyBudget.Next.Data;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudget.Next.Controllers
{
    public class HomeController : Controller
    {
        private readonly FamilyBudgetEntities _context;

        public HomeController(FamilyBudgetEntities context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
