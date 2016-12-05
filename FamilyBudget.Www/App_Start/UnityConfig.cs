using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_CodeBase.Csv;
using FamilyBudget.Www.Areas.Administration.Controllers;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Models.Repository;
using FamilyBudget.Www.Models.Repository.Interfaces;
using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;

namespace FamilyBudget.Www
{
    public static class UnityConfig
    {
        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();

            RegisterControllers(container);
            RegisterApiControllers(container);

            container.RegisterType(typeof(CurrencyRatePreloader));
            //container.RegisterType(typeof(IncomeApiController));

            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            return container;
        }

        private static void RegisterApiControllers(IUnityContainer container)
        {
            container.RegisterType(typeof(Controllers.Api.IncomeController));
        }

        private static void RegisterControllers(IUnityContainer container)
        {
            container.RegisterType(typeof(ISiteMapProvider), typeof(SpaSiteMapProvider));
            container.RegisterType(typeof(PageContextActionFilter), typeof(PageContextActionFilter));

            container.RegisterType(typeof(ICurrencyProvider), typeof(YahooCurrencyProvider));
            container.RegisterType(typeof(IGoldOunceRateProvider), typeof(GoldOunceRateProvider));
            //container.RegisterType(typeof(ICurrencyProvider), typeof(CBRCurrencyProvider));

            container.RegisterType(typeof(IProsperityProvider), typeof(ProsperityProvider));

            container.RegisterType(typeof(IAccountRepository), typeof(AccountRepository));
            container.RegisterType(typeof(ICurrencyRepository), typeof(CurrencyRepository));
            container.RegisterType(typeof(IExpenditureCategoryRepository), typeof(ExpenditureCategoryRepository));
            container.RegisterType(typeof(IExpenditureRepository), typeof(ExpenditureRepository));
            container.RegisterType(typeof(IIncomeCategoryRepository), typeof(IncomeCategoryRepository));
            container.RegisterType(typeof(IIncomeRepository), typeof(IncomeRepository));

            container.RegisterType(typeof(HomeController));
            container.RegisterType(typeof(ExpenditureController));
            container.RegisterType(typeof(Controllers.IncomeController));
            container.RegisterType(typeof(ReportController));
            container.RegisterType(typeof(UserController));
            container.RegisterType(typeof(AccountController));
            container.RegisterType(typeof(CurrencyController));
            container.RegisterType(typeof(ExpenditureCategoryController));
            container.RegisterType(typeof(IncomeCategoryController));
            container.RegisterType(typeof(ErrorHandlerController));
        }
    }
}