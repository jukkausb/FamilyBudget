using FamilyBudget.v3.App_CodeBase.Tinkoff;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.App_Utils;
using FamilyBudget.v3.Controllers.Services;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
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
        private readonly ITinkoffInvestmentDataProvider _tinkoffInvestmentDataProvider;

        public HomeController(IAccountRepository accountRepository, IIncomeRepository incomeRepository,
            IExpenditureRepository expenditureRepository, ICalculationService calculationService,
            ITinkoffInvestmentDataProvider tinkoffInvestmentDataProvider)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _calculationService = calculationService;
            _tinkoffInvestmentDataProvider = tinkoffInvestmentDataProvider;
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

            var totalCash = _calculationService.CalculateCash(mainCurrencyCode);
            model.Cash = new MoneyModel
            {
                Currency = mainCurrencyCode.ToCurrencySymbol(),
                Value = totalCash,
                ValuePresentation = totalCash.ToCurrencyDisplay(mainCurrencyCode, true)
            };

            List<InvestmentAccount> investmentAccounts = null;

            try
            {
                investmentAccounts = _tinkoffInvestmentDataProvider.GetInvestmentAccounts();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                model.Message = new MessageModel();
                model.Message.Messages.Add(new Message
                {
                    Text = "Ошибка загрузки данных с Tinkoff",
                    Type = MessageType.Error
                });
            }

            var totalInvestmentBalance = investmentAccounts?.Sum(a => a.TotalBalance) ?? 0;

            model.Investment = new MoneyModel
            {
                Currency = mainCurrencyCode.ToCurrencySymbol(),
                Value = totalInvestmentBalance,
                ValuePresentation = totalInvestmentBalance.ToCurrencyDisplay(mainCurrencyCode, true)
            };

            var totalCapital = totalInvestmentBalance + totalCash;
            model.Capital = new MoneyModel
            {
                Currency = mainCurrencyCode.ToCurrencySymbol(),
                Value = totalCapital,
                ValuePresentation = totalCapital.ToCurrencyDisplay(mainCurrencyCode, true)
            };

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