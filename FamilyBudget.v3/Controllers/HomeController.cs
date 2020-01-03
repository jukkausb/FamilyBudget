using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_CodeBase.Widgets;
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
        private readonly ITrendCalculator _trendCalculator;

        public HomeController(IAccountRepository accountRepository, IIncomeRepository incomeRepository, IExpenditureRepository expenditureRepository,
            ICurrencyProvider currencyProvider, ITrendCalculator trendCalculator)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _currencyProvider = currencyProvider;
            _trendCalculator = trendCalculator;
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

            // Average month income
            DateTime endDate = DateTime.Now;
            int lastMonthCount = 6;
            DateTime startDate = endDate.AddMonths(-lastMonthCount);

            model.AverageLastMonthCount = lastMonthCount;

            var realImcomes = BusinessHelper.GetRealIncomes(_incomeRepository);
            var averageMonthIncome = (from income in realImcomes
                                           where income.AccountID == accountRub.ID
                                           where income.Date >= startDate.Date && income.Date <= endDate.Date
                                           group income by new { income.Date.Month, income.Date.Year } into g
                                           select new
                                           {
                                               Period = new Period { Year = g.Key.Year, Month = g.Key.Month },
                                               IncomeTotal = g.Sum(i => i.Summa)
                                           }).Average(e => e.IncomeTotal);

            model.AverageIncomePerMonth = new MoneyModel
            {
                Value = averageMonthIncome
            };

            model.AccountRateViews = accountRateViews;

            // Average month expenditure
            var realExpenditures = BusinessHelper.GetRealExpenditures(_expenditureRepository);
            var averageMonthExpenditure = (from expenditure in realExpenditures
                                           where expenditure.AccountID == accountRub.ID
                    where expenditure.Date >= startDate.Date && expenditure.Date <= endDate.Date
                    group expenditure by new { expenditure.Date.Month, expenditure.Date.Year } into g
                    select new
                    {
                        Period = new Period { Year = g.Key.Year, Month = g.Key.Month },
                        ExpenditureTotal = g.Sum(i => i.Summa)
                    }).Average(e => e.ExpenditureTotal);

            model.AverageExpenditurePerMonth = new MoneyModel
            {
                Value = averageMonthExpenditure
            };

            model.AverageProfitPerMonth = new MoneyModel
            {
                Value = averageMonthIncome - averageMonthExpenditure
            };

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

    public class MonthDefinition
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class TrendLineMonthDefinition : MonthDefinition
    {
        public decimal Value { get; set; }
        public decimal TrendValue { get; set; }
    }

    class MonthDefinitionComparer : IEqualityComparer<MonthDefinition>
    {
        public bool Equals(MonthDefinition b1, MonthDefinition b2)
        {
            if (b2 == null || b1 == null)
                return false;

            return b1.Month == b2.Month && b1.Year == b2.Year;
        }

        public int GetHashCode(MonthDefinition bx)
        {
            int hCode = bx.Year ^ bx.Month;
            return hCode.GetHashCode();
        }
    }
}