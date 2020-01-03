using System.Web;
using System.Web.Mvc;
using FamilyBudget.v3.Models.Spa.Shared;

namespace FamilyBudget.v3.App_CodeBase
{
    public class PageContextActionFilter : IActionFilter
    {
        private readonly ISiteMapProvider _siteMapProvider;
        public PageContextActionFilter(ISiteMapProvider siteMapProvider)
        {
            _siteMapProvider = siteMapProvider;
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model;
            PageViewModel model = viewModel as PageViewModel;
            if (model != null && model.PageHeader == null)
            {
                SiteMap siteMap = _siteMapProvider.GetSiteMap();
                SiteNavigationViewModel siteNavigationModel = _siteMapProvider.GetSiteNavigationViewModel(siteMap);
                model.SiteNavigation = siteNavigationModel;

                model.PageHeader = new PageHeaderViewModel
                {
                    IsLoggedIn = HttpContext.Current.User.Identity.IsAuthenticated
                };

                string userName = "Anna";
                model.PageHeader.UserName = userName;

                //TODO: Footer model
            }
        }
    }
}