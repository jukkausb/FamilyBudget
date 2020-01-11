using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Controllers.Services;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FamilyBudget.v3.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly ICalculationService _calculationService;

        public HomeController(IAccountRepository accountRepository, IIncomeRepository incomeRepository, 
            IExpenditureRepository expenditureRepository, ICalculationService calculationService)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _calculationService = calculationService;
        }

        public ActionResult Index()
        {
            DashboardModel model = new DashboardModel();

            var accounts = _accountRepository.GetAll().ToList();
            Account mainAccount = accounts.FirstOrDefault(a => a.IsMain);
            string mainCurrencyCode = mainAccount.Currency.Code;

            var accountRub = accounts.FirstOrDefault(a => a.Currency.Code == "RUB");
            var accountUsd = accounts.FirstOrDefault(a => a.Currency.Code == "USD");
            var accountEur = accounts.FirstOrDefault(a => a.Currency.Code == "EUR");

            if (mainAccount != null)
            {
                model.Wealth = new MoneyModel
                {
                    Currency = mainCurrencyCode.ToCurrencySymbol(),
                    Value = _calculationService.CalculateWealth(mainCurrencyCode)
                };
            }

            model.AccountRateViews = _calculationService.GetAccountBalanceWithRatesViews();

            #region Average values

            DateTime endDate = DateTime.Now.AddMonths(-1); // Do not include current month
            int lastMonthCount = 6;
            DateTime startDate = endDate.AddMonths(-lastMonthCount);

            model.AverageLastMonthCount = lastMonthCount;

            var realImcomes = BusinessHelper.GetRealIncomes(_incomeRepository);
            var allIncomesInPeriod = realImcomes.Where(i => i.Date >= startDate.Date && i.Date <= endDate.Date).ToList();
            var realExpenditures = BusinessHelper.GetRealExpenditures(_expenditureRepository);
            var allExpendituresInPeriod = realExpenditures.Where(i => i.Date >= startDate.Date && i.Date <= endDate.Date).ToList();

            var averageMonthModel = _calculationService.CalculateAverageValues(accounts, allIncomesInPeriod, allExpendituresInPeriod, mainCurrencyCode, lastMonthCount);

            model.AverageIncomePerMonth = averageMonthModel.AverageIncome;
            model.AverageExpenditurePerMonth = averageMonthModel.AverageExpenditure;
            model.AverageProfitPerMonth = averageMonthModel.AverageNetProfit;

            #endregion

            #region Investment

            decimal allToIIS = BusinessHelper.GetIISExpenditures(_expenditureRepository).Sum(e => e.Summa);
            model.AllIISExpenditureTotal = new MoneyModel
            {
                Value = allToIIS
            };

            decimal allToBrokerAccount = BusinessHelper.GetBrokerAccountExpenditures(_expenditureRepository).Sum(e => e.Summa);
            model.AllBrokerAccountExpenditureTotal = new MoneyModel
            {
                Value = allToBrokerAccount
            };

            #endregion

            return View(model);
        }
    }
}