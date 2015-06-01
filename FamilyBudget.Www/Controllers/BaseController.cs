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

namespace FamilyBudget.Www.Controllers
{
    [Authorize]
    [LayoutInjecter("_Layout")]
    [HandleError]
    public class BaseController : Controller
    {
        protected FamilyBudgetEntities DbModelFamilyBudgetEntities = new FamilyBudgetEntities();

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
                NumberFormat = {NumberDecimalSeparator = "."}
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DbModelFamilyBudgetEntities.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Business Logic

        protected List<SelectListItem> GetCurrencies()
        {
            List<SelectListItem> currencies =
                DbModelFamilyBudgetEntities.Currency.ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0} ({1})", c.Name, c.Code)
                }).ToList();

            currencies.Insert(0, new SelectListItem {Text = " - Выберите валюту - ", Value = ""});
            return currencies;
        }

        protected List<Account> GetAccountsData()
        {
            return DbModelFamilyBudgetEntities.Account.ToList();
        }

        protected List<ExtendedSelectListItem> GetAccountsForDropDownExtended()
        {
            List<ExtendedSelectListItem> accounts =
                DbModelFamilyBudgetEntities.Account.ToList().Select(c => new ExtendedSelectListItem
                {
                    IsBold = c.IsMain,
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = c.DisplayName,
                    Selected = c.IsMain,
                    HtmlAttributes = new {data_currency = c.Currency.Code}
                }).ToList();

            accounts.Insert(0, new ExtendedSelectListItem {Text = " - Выберите счет - ", Value = ""});
            return accounts;
        }

        protected List<SelectListItem> GetExpenditureCategories()
        {
            List<SelectListItem> categories =
                DbModelFamilyBudgetEntities.ExpenditureCategory.ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0}", c.Name)
                }).ToList();

            categories.Insert(0, new SelectListItem {Text = " - Выберите категорию - ", Value = ""});
            return categories;
        }

        protected List<SelectListItem> GetIncomeCategories()
        {
            List<SelectListItem> categories =
                DbModelFamilyBudgetEntities.IncomeCategory.ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0}", c.Name)
                }).ToList();

            categories.Insert(0, new SelectListItem {Text = " - Выберите категорию - ", Value = ""});
            return categories;
        }

        #endregion
    }
}