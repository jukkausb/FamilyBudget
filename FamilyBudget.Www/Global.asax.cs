using System.Web.Http;
using FamilyBudget.Www.App_CodeBase;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FamilyBudget.Www.App_Start;
using FamilyBudget.Www.Models.Repository.Interfaces;
using Microsoft.Practices.Unity;
using System.Net;

namespace FamilyBudget.Www
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            IUnityContainer container = UnityConfig.RegisterComponents();

            AreaRegistration.RegisterAllAreas();

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            JsonConfig.SetSerializerSettings(GlobalConfiguration.Configuration);

            PreloadRates(container);
        }

        private static void PreloadRates(IUnityContainer container)
        {
            (new CurrencyRatePreloader(
                container.Resolve<IAccountRepository>(),
                container.Resolve<ICurrencyProvider>())).Preload();

            (new GoldOunceRatePreloader(container.Resolve<IGoldOunceRateProvider>())).Preload();
        }
    }
}