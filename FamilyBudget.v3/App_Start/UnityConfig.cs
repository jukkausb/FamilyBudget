using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_CodeBase.Csv;
using FamilyBudget.v3.App_CodeBase.Json;
using FamilyBudget.v3.App_CodeBase.Tinkoff;
using FamilyBudget.v3.Areas.Administration.Controllers;
using FamilyBudget.v3.Controllers;
using FamilyBudget.v3.Controllers.Services;
using FamilyBudget.v3.Models.Repository;
using FamilyBudget.v3.Models.Repository.Interfaces;
using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;

namespace FamilyBudget.v3
{
    public static class UnityConfig
    {
        public static IUnityContainer RegisterComponents()
        {
            var container = new UnityContainer();

            RegisterControllers(container);

            container.RegisterType(typeof(CurrencyRatePreloader));

            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            return container;
        }

        private static void RegisterControllers(IUnityContainer container)
        {
            container.RegisterType(typeof(ITinkoffInvestmentDataProvider), typeof(TinkoffInvestmentDataProvider));

            //container.RegisterType(typeof(ICurrencyProvider), typeof(YahooCurrencyProvider));
            container.RegisterType(typeof(ICurrencyProvider), typeof(FreeCurrencyConverter));
            container.RegisterType(typeof(IGoldOunceRateProvider), typeof(GoldOunceRateProvider));
            //container.RegisterType(typeof(ICurrencyProvider), typeof(CBRCurrencyProvider));

            container.RegisterType(typeof(ICalculationService), typeof(CalculationService));

            container.RegisterType(typeof(IAccountRepository), typeof(AccountRepository));
            container.RegisterType(typeof(ICurrencyRepository), typeof(CurrencyRepository));
            container.RegisterType(typeof(IExpenditureCategoryRepository), typeof(ExpenditureCategoryRepository));
            container.RegisterType(typeof(IExpenditureRepository), typeof(ExpenditureRepository));
            container.RegisterType(typeof(IIncomeCategoryRepository), typeof(IncomeCategoryRepository));
            container.RegisterType(typeof(IIncomeRepository), typeof(IncomeRepository));
            container.RegisterType(typeof(IInvestmentRulesInstrumentsRepository), typeof(InvestmentRulesInstrumentsRepository));
            container.RegisterType(typeof(IInvestmentRulesEtfRepository), typeof(InvestmentRulesEtfRepository));

            container.RegisterType(typeof(IIncomeSuggestionService), typeof(IncomeSuggestionService));
            container.RegisterType(typeof(IExpenditureSuggestionService), typeof(ExpenditureSuggestionService));

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