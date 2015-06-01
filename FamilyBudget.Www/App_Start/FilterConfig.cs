using System.Web.Mvc;
using FamilyBudget.Www.App_CodeBase;

namespace FamilyBudget.Www
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleExceptionAttribute());
        }
    }
}