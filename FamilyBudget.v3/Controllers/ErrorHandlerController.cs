using FamilyBudget.v3.App_CodeBase;
using System;
using System.Web.Mvc;

namespace FamilyBudget.v3.Controllers
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