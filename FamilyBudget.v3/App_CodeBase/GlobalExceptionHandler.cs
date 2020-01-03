
using System;
using System.Web;

namespace FamilyBudget.v3.App_CodeBase
{
    public static class GlobalExceptionHandler
    {
        public const string GlobalExceptionKey = "GlobalExceptionKey";

        public static void SetApplicationWarning(Exception exception)
        {
            HttpContext.Current.Application.Add(GlobalExceptionKey, exception);
        }

        public static Exception GetApplicationWarning()
        {
            return (Exception)HttpContext.Current.Application.Get(GlobalExceptionKey);
        }

        public static void RemoveApplicationWarning()
        {
            HttpContext.Current.Application.Remove(GlobalExceptionKey);
        }
    }
}