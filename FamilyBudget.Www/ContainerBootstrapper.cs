using FamilyBudget.Www.Areas.Administration.Controllers;
using FamilyBudget.Www.CallHandlers;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Repository;
using FamilyBudget.Www.Repository.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FamilyBudget.Www
{
    public static class ContainerBootstrapper
    {
        public static void Configure()
        {
            var container = new UnityContainer();

            InjectionMember[] injectionMembers = { new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>() };

            container.RegisterType(typeof(IAccountRepository), typeof(AccountRepository), injectionMembers);
            container.RegisterType(typeof(ICurrencyRepository), typeof(CurrencyRepository), injectionMembers);
            container.RegisterType(typeof(IExpenditureCategoryRepository), typeof(ExpenditureCategoryRepository), injectionMembers);
            container.RegisterType(typeof(IExpenditureRepository), typeof(ExpenditureRepository), injectionMembers);
            container.RegisterType(typeof(IIncomeCategoryRepository), typeof(IncomeCategoryRepository), injectionMembers);
            container.RegisterType(typeof(IIncomeRepository), typeof(IncomeRepository), injectionMembers);

            container.RegisterType(typeof(HomeController));
            container.RegisterType(typeof(ExpenditureController));
            container.RegisterType(typeof(IncomeController));
            container.RegisterType(typeof(ReportController));
            container.RegisterType(typeof(UserController));
            container.RegisterType(typeof(AccountController));
            container.RegisterType(typeof(CurrencyController));
            container.RegisterType(typeof(ExpenditureCategoryController));
            container.RegisterType(typeof(IncomeCategoryController));

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