using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace FamilyBudget.v3.Controllers
{
    [LayoutInjecter("_LayoutLogin")]
    public class UserController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool userValid = Membership.ValidateUser(model.UserName, model.Password);
                if (userValid)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return Redirect("/Home");
                }
                {
                    ModelState.AddModelError("", "Логин/пароль указаны неверно");
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}