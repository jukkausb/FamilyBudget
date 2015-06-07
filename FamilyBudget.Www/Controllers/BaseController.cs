using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Utils;
using FamilyBudget.Www.Repository.Interfaces;
using Microsoft.Practices.Unity;

namespace FamilyBudget.Www.Controllers
{
    [Authorize]
    [LayoutInjecter("_Layout")]
    [HandleError]
    public class BaseController : Controller
    {
        /// <summary>
        ///     Default error view name
        /// </summary>
        public virtual string ErrorViewName
        {
            get { return "Error"; }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            return new JsonDotNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var culture = new CultureInfo("ru-RU")
            {
                NumberFormat = { NumberDecimalSeparator = "." }
            };

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        ///     Handles exception and returns error view
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Error view</returns>
        protected ViewResult HandleException(Exception ex)
        {
            Logger.Error(ex);
            return View(ErrorViewName, ex.Message);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Logger.Error(filterContext.Exception);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult() { ViewName = ErrorViewName, MasterName = "_Layout" };
        }
    }
}