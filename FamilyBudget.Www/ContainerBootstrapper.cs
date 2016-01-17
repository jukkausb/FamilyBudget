using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_CodeBase.Csv;
using FamilyBudget.Www.Areas.Administration.Controllers;
using FamilyBudget.Www.CallHandlers;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Models.Repository;
using FamilyBudget.Www.Models.Repository.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.Web.Mvc;

namespace FamilyBudget.Www
{
    public static class ContainerBootstrapper
    {
        public static void Configure()
        {
            var container = new UnityContainer();

            InjectionMember[] injectionMembers = { new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>() };
            InjectionMember[] injectionMembersType = { new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<VirtualMethodInterceptor>() };

            container.RegisterType(typeof(ICurrencyProvider), typeof(YahooCurrencyProvider), injectionMembers);
            container.RegisterType(typeof(IGoldOunceRateProvider), typeof(GoldOunceRateProvider), injectionMembers);
            //container.RegisterType(typeof(ICurrencyProvider), typeof(CBRCurrencyProvider), injectionMembers);

            container.RegisterType(typeof(IProsperityProvider), typeof(ProsperityProvider), injectionMembers);

            container.RegisterType(typeof(IAccountRepository), typeof(AccountRepository), injectionMembers);
            container.RegisterType(typeof(ICurrencyRepository), typeof(CurrencyRepository), injectionMembers);
            container.RegisterType(typeof(IExpenditureCategoryRepository), typeof(ExpenditureCategoryRepository), injectionMembers);
            container.RegisterType(typeof(IExpenditureRepository), typeof(ExpenditureRepository), injectionMembers);
            container.RegisterType(typeof(IIncomeCategoryRepository), typeof(IncomeCategoryRepository), injectionMembers);
            container.RegisterType(typeof(IIncomeRepository), typeof(IncomeRepository), injectionMembers);

            container.RegisterType(typeof(HomeController), injectionMembersType);
            container.RegisterType(typeof(ExpenditureController), injectionMembersType);
            container.RegisterType(typeof(IncomeController), injectionMembersType);
            container.RegisterType(typeof(ReportController), injectionMembersType);
            container.RegisterType(typeof(UserController), injectionMembersType);
            container.RegisterType(typeof(AccountController), injectionMembersType);
            container.RegisterType(typeof(CurrencyController), injectionMembersType);
            container.RegisterType(typeof(ExpenditureCategoryController), injectionMembersType);
            container.RegisterType(typeof(IncomeCategoryController), injectionMembersType);
            container.RegisterType(typeof(ErrorHandlerController), injectionMembersType);

            container.AddNewExtension<Interception>()
                .Configure<Interception>()
                .AddPolicy("logging")
                .AddMatchingRule<AssemblyMatchingRule>(new InjectionConstructor(new InjectionParameter("FamilyBudget.Www")))
                .AddCallHandler<LoggingCallHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor(), new InjectionProperty("Order", 1)); 

            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(container)); 
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }
    }
}