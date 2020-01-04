using System.Web.Mvc;
using FamilyBudget.v3.App_CodeBase;

namespace FamilyBudget.v3
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleExceptionAttribute());
        }
    }
}