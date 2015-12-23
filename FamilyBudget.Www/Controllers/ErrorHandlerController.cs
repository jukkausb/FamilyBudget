using FamilyBudget.Www.App_CodeBase;
using System;
using System.Web.Mvc;

namespace FamilyBudget.Www.Controllers
{
    public class ErrorHandlerController : Controller
    {
        public ActionResult Index()
        {
            Exception warning = GlobalExceptionHandler.GetApplicationWarning();
            return warning != null ? PartialView("Warning", warning.Message) : null;
        }
    }
}