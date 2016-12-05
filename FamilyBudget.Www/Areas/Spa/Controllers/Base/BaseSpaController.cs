using System.Web.Mvc;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.Models.Spa.Shared;

namespace FamilyBudget.Www.Areas.Spa.Controllers.Base
{
    [LayoutInjecter("_LayoutSpa")]
    public abstract class BaseSpaController : Controller
    {
        protected abstract string AppName { get; }

        public ActionResult Index()
        {
            PageViewModel model = new PageViewModel();
            model.AppName = AppName;
            return View("Index", model);
        }
    }
}