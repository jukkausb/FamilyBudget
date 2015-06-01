using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using FamilyBudget.Www.Models;

namespace FamilyBudget.Www.Controllers
{
    public class UserController : BaseController
    {
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([FromBody] LoginModel model, string returnUrl)
        {
            double s = DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds;

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