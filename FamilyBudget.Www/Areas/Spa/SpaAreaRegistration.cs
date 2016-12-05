using System.Web.Mvc;

namespace FamilyBudget.Www.Areas.Spa
{
    public class SpaAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Spa";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Spa_default",
                "Spa/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional, controller = "Default" }
            );
        }
    }
}