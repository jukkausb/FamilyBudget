using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Utils;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Home;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FamilyBudget.v3.Controllers.Services
{
    public interface ICalculationService
    {
        decimal CalculateWealth(string mainCurrencyCode);
        AverageMoneyModel CalculateAverageValues(List<Account> accounts,
            List<Income> allIncomesInMonth,
            List<Expenditure> allExpenditresInMonth,
            string mainCurrencyCode,
            int monthCount);
        List<AccountRateView> GetAccountBalanceWithRatesViews();
    }

    public class CalculationService : ICalculationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrencyProvider _currencyProvider;

        public CalculationService(IAccountRepository accountRepository, ICurrencyProvider currencyProvider)
        {
            _accountRepository = accountRepository;
            _currencyProvider = currencyProvider;
        }

        public decimal CalculateWealth(string mainCurrencyCode)
        {
            decimal wealthValue = 0;

            var accountData = _accountRepository.GetAll().Select(a => new
            {
                CurrencyCode = a.Currency.Code,
                a.Balance
            }).ToList();

            accountData.ForEach(a =>
            {
                if (a.CurrencyCode != mainCurrencyCode)
                {
                    decimal rate = _currencyProvider.GetSellCurrencyRate(a.CurrencyCode, mainCurrencyCode);
                    wealthValue += a.Balance * rate;
                }
                else
                {
                    wealthValue += a.Balance;
                }
            });

            return wealthValue;
        }

        public AverageMoneyModel CalculateAverageValues(List<Account> accounts,
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

        public List<AccountRateView> GetAccountBalanceWithRatesViews()
        {
            var accounts = _accountRepository.GetAll().ToList();
            Account mainAccount = accounts.FirstOrDefault(a => a.IsMain);
            string mainCurrencyCode = mainAccount.Currency.Code;
            var wealth = CalculateWealth(mainCurrencyCode);

            var accountRub = accounts.FirstOrDefault(a => a.Currency.Code == Constants.CURRENCY_RUB);
            var accountUsd = accounts.FirstOrDefault(a => a.Currency.Code == Constants.CURRENCY_USD);
            var accountEur = accounts.FirstOrDefault(a => a.Currency.Code == Constants.CURRENCY_EUR);

            List<AccountRateView> accountRateViews = new List<AccountRateView>();

            var rubAccountView = new AccountRateView
            {
                Name = accountRub.Name,
                Balance = accountRub.Balance,
                CurrencyCode = accountRub.Currency.Code,
                Rate = 1
            };
            rubAccountView.Percent = rubAccountView.Equivalent / wealth * 100;
            accountRateViews.Add(rubAccountView);

            var usdAccountView = new AccountRateView
            {
                Name = accountUsd.Name,
                Balance = accountUsd.Balance,
                CurrencyCode = accountUsd.Currency.Code,
                Rate = _currencyProvider.GetSellCurrencyRate(accountUsd.Currency.Code, mainCurrencyCode),
                Percent = accountUsd.Balance / wealth
            };
            usdAccountView.Percent = usdAccountView.Equivalent / wealth * 100;
            accountRateViews.Add(usdAccountView);

            var eurAccountView = new AccountRateView
            {
                Name = accountEur.Name,
                Balance = accountEur.Balance,
                CurrencyCode = accountEur.Currency.Code,
                Rate = _currencyProvider.GetSellCurrencyRate(accountEur.Currency.Code, mainCurrencyCode)
            };
            eurAccountView.Percent = eurAccountView.Equivalent / wealth * 100;
            accountRateViews.Add(eurAccountView);

            return accountRateViews;
        }
    }
}