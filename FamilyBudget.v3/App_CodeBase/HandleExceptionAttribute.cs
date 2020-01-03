using System.Web.Mvc;
using FamilyBudget.v3.App_Utils;

namespace FamilyBudget.v3.App_CodeBase
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