using FamilyBudget.Www.Areas.Administration.Controllers;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Repository;
using FamilyBudget.Www.Repository.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
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

            container.RegisterType(typeof(IAccountRepository), typeof(AccountRepository));
            container.RegisterType(typeof(ICurrencyRepository), typeof(CurrencyRepository));
            container.RegisterType(typeof(IExpenditureCategoryRepository), typeof(ExpenditureCategoryRepository));
            container.RegisterType(typeof(IExpenditureRepository), typeof(ExpenditureRepository));
            container.RegisterType(typeof(IIncomeCategoryRepository), typeof(IncomeCategoryRepository));
            container.RegisterType(typeof(IIncomeRepository), typeof(IncomeRepository));

            container.RegisterType(typeof(HomeController));
            container.RegisterType(typeof(ExpenditureController));
            container.RegisterType(typeof(IncomeController));
            container.RegisterType(typeof(ReportController));
            container.RegisterType(typeof(UserController));

            container.RegisterType(typeof(AccountController));
            container.RegisterType(typeof(CurrencyController));
            container.RegisterType(typeof(ExpenditureCategoryController));
            container.RegisterType(typeof(IncomeCategoryController));


            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(container)); 
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }
    }
}