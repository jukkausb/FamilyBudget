using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.App_Utils;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Home;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FamilyBudget.v3.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenditureRepository _expenditureRepository;

        public HomeController(IAccountRepository accountRepository, IIncomeRepository incomeRepository, 
            IExpenditureRepository expenditureRepository, ICurrencyProvider currencyProvider)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _currencyProvider = currencyProvider;
        }

        public ActionResult Index()
        {
            DashboardModel model = new DashboardModel();

            var accounts = _accountRepository.GetAll().ToList();
            Account mainAccount = accounts.FirstOrDefault(a => a.IsMain);
            var accountRub = accounts.FirstOrDefault(a => a.Currency.Code == "RUB");
            var accountUsd = accounts.FirstOrDefault(a => a.Currency.Code == "USD");
            var accountEur = accounts.FirstOrDefault(a => a.Currency.Code == "EUR");

            if (mainAccount != null)
            {
                string mainAccountCurrencyCode = mainAccount.Currency.Code;

                model.Wealth = new MoneyModel
                {
                    Currency = mainAccountCurrencyCode.ToCurrencySymbol(),
                    Value = CalculateWealth(mainAccountCurrencyCode)
                };
                model.DollarWealth = new MoneyModel
                {
                    Value = accountUsd.Balance
                };
                model.RubleWealth = new MoneyModel
                {
                    Value = accountRub.Balance
                };
                model.EuroWealth = new MoneyModel
                {
                    Value = accountEur.Balance
                };
            }

            string mainCurrencyCode = mainAccount.Currency.Code;

            List<AccountRateViewNew> accountRateViews = new List<AccountRateViewNew>();

            var usdAccount = accounts.FirstOrDefault(a => a.Currency.Code == "USD");
            accountRateViews.Add(new AccountRateViewNew
            {
                Balance = usdAccount.Balance,
                CurrencyCode = usdAccount.Currency.Code,
                Rate = _currencyProvider.GetSellCurrencyRate(usdAccount.Currency.Code, mainCurrencyCode)
            });

            var euroAccount = accounts.FirstOrDefault(a => a.Currency.Code == "EUR");
            accountRateViews.Add(new AccountRateViewNew
            {
                Balance = euroAccount.Balance,
                CurrencyCode = euroAccount.Currency.Code,
                Rate = _currencyProvider.GetSellCurrencyRate(euroAccount.Currency.Code, mainCurrencyCode)
            });

            model.AccountRateViews = accountRateViews;

            // Average month income
            DateTime endDate = DateTime.Now;
            int lastMonthCount = 6;
            DateTime startDate = endDate.AddMonths(-lastMonthCount);

            model.AverageLastMonthCount = lastMonthCount;

            var realImcomes = BusinessHelper.GetRealIncomes(_incomeRepository);
            var allIncomesInPeriod = realImcomes.Where(i => i.Date >= startDate.Date && i.Date <= endDate.Date).ToList();
            var realExpenditures = BusinessHelper.GetRealExpenditures(_expenditureRepository);
            var allExpendituresInPeriod = realExpenditures.Where(i => i.Date >= startDate.Date && i.Date <= endDate.Date).ToList();

            var averageMonthModel = CalculateNetProfit(accounts, allIncomesInPeriod, allExpendituresInPeriod, mainCurrencyCode, lastMonthCount);

            model.AverageIncomePerMonth = averageMonthModel.AverageIncome;
            model.AverageExpenditurePerMonth = averageMonthModel.AverageExpenditure;
            model.AverageProfitPerMonth = averageMonthModel.AverageNetProfit;

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


            return View(model);
        }

        private AverageMoneyModel CalculateNetProfit(List<Account> accounts,
            List<Income> allIncomesInMonth,
            List<Expenditure> allExpenditresInMonth,
            string mainCurrencyCode,
            int monthCount)
        {
            AverageMoneyModel model = new AverageMoneyModel();
            decimal totalMonthIncome = 0;
            decimal totalMonthExpenditure = 0;

            accounts.ForEach(a =>
            {
                var accountCurrencyCode = a.Currency.Code;
                var accountId = a.ID;

                decimal rate = accountCurrencyCode != mainCurrencyCode ? _currencyProvider.GetSellCurrencyRate(accountCurrencyCode, mainCurrencyCode) : 1;
                Logger.Info(string.Format("Exchange rate ({0}-{1}): {2}", a.Currency.Code, mainCurrencyCode, rate));

                totalMonthIncome += (from income in allIncomesInMonth
                                     where income.AccountID == a.ID
                                     group income by new { income.Date.Month, income.Date.Year } into g
                                     select new
                                     {
                                         IncomeTotal = accountCurrencyCode != mainCurrencyCode ? g.Sum(i => i.Summa * rate) : g.Sum(i => i.Summa)
                                     }).Sum(e => e.IncomeTotal);

                totalMonthExpenditure += (from expenditure in allExpenditresInMonth
                                          where expenditure.AccountID == a.ID
                                          group expenditure by new { expenditure.Date.Month, expenditure.Date.Year } into g
                                          select new
                                          {
                                              ExpenditureTotal = accountCurrencyCode != mainCurrencyCode ? g.Sum(i => i.Summa * rate) : g.Sum(i => i.Summa)
                                          }).Sum(e => e.ExpenditureTotal); ;
            }
            );

            model.AverageExpenditure = new MoneyModel { Value = totalMonthExpenditure / monthCount };
            model.AverageIncome = new MoneyModel { Value = totalMonthIncome / monthCount };
            model.AverageNetProfit = new MoneyModel { Value = (totalMonthIncome - totalMonthExpenditure) / monthCount };

            return model;
        }

        private decimal CalculateWealth(string mainCurrencyCode)
        {
            decimal wealthValue = 0;
            List<Account> accounts = _accountRepository.GetAll().ToList();
            accounts.ForEach(a =>
            {
                if (a.Currency.Code != mainCurrencyCode)
                {
                    decimal rate = _currencyProvider.GetSellCurrencyRate(a.Currency.Code, mainCurrencyCode);
                    Logger.Info(string.Format("Exchange rate ({0}-{1}): {2}", a.Currency.Code, mainCurrencyCode, rate));
                    wealthValue += a.Balance * rate;
                }
                else
                {
                    wealthValue += a.Balance;
                }
            });

            return wealthValue;
        }
    }
}