using System.Web.Mvc;
using FamilyBudget.Www.App_Utils;

namespace FamilyBudget.Www.App_CodeBase
{
    public class HandleExceptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Logger.Error(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    MasterName = "_Layout"
                };
            }
        }
    }
}