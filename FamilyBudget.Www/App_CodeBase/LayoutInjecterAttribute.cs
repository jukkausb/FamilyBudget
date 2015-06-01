﻿using System.Web.Mvc;

namespace FamilyBudget.Www.App_CodeBase
{
    public class LayoutInjecterAttribute : ActionFilterAttribute
    {
        private readonly string _masterName;

        public LayoutInjecterAttribute(string masterName)
        {
            _masterName = masterName;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var result = filterContext.Result as ViewResult;
            if (result != null)
            {
                result.MasterName = _masterName;
            }
        }
    }
}