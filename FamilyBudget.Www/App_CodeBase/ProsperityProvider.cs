using System;
using System.Collections.Generic;
using System.Linq;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Utils;
using FamilyBudget.Www.Models.Repository.Interfaces;

namespace FamilyBudget.Www.App_CodeBase
{
    public class ProsperityProvider : IProsperityProvider
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IGoldOunceRateProvider _goldOunceRateProvider;

        public ProsperityProvider(IAccountRepository accountRepository, IIncomeRepository incomeRepository, IExpenditureRepository expenditureRepository, IGoldOunceRateProvider goldOunceRateProvider)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;
            _goldOunceRateProvider = goldOunceRateProvider;

            // Only CBRCurrencyProvider can get rates for periods in the past
            _currencyProvider = new CBRCurrencyProvider();
        }

        public Prosperity GetProsperity(int month, int year)
        {
            var lastDayOfThePeriod = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var allIncomesUntilRequestedPeriod = _incomeRepository.FindBy(i => i.Date <= lastDayOfThePeriod.Date).ToList();
            var allExpenditresUntilRequestedPeriod = _expenditureRepository.FindBy(i => i.Date <= lastDayOfThePeriod.Date).ToList();

            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain || a.Currency.Code.ToUpper() == "RUB");
            if (mainAccount == null)
                throw new Exception("Main account not found");

            string mainCurrencyCode = mainAccount.Currency.Code;
            decimal wealth = CalculateWealth(allIncomesUntilRequestedPeriod, allExpenditresUntilRequestedPeriod, mainCurrencyCode);

            decimal USDTORUBrate = _currencyProvider.GetSellCurrencyRate("USD", "RUB");
            var goldOunceRate = _goldOunceRateProvider.GetAverageGoldOunceRate("USD", year, month) * USDTORUBrate;

            decimal prosperityValue = wealth / goldOunceRate;

            return new Prosperity()
            {
                Incomes = allIncomesUntilRequestedPeriod,
                Expenditures = allExpenditresUntilRequestedPeriod,
                Month = month,
                Year = year,
                Wealth = wealth,
                ProsperityValue = prosperityValue,
                GoldOunce = goldOunceRate
            };
        }

        private decimal CalculateWealth(List<Income> allIncomesUntilRequestedPeriod, List<Expenditure> allExpenditresUntilRequestedPeriod, string mainCurrencyCode)
        {
            decimal wealthValue = 0;
            List<Account> accounts = _accountRepository.GetAll().ToList();
            accounts.ForEach(a =>
            {
                var accountCurrencyCode = a.Currency.Code;
                var accountId = a.ID;

                decimal rate = accountCurrencyCode != mainCurrencyCode ? _currencyProvider.GetSellCurrencyRate(accountCurrencyCode, mainCurrencyCode) : 1;
                Logger.Info(string.Format("Exchange rate ({0}-{1}): {2}", a.Currency.Code, mainCurrencyCode, rate));

                List<Income> allIncomesUntilRequestedPeriodInMainCurrency = null;
                if (allIncomesUntilRequestedPeriod != null && allIncomesUntilRequestedPeriod.Any())
                {
                    allIncomesUntilRequestedPeriodInMainCurrency = allIncomesUntilRequestedPeriod.Where(i => i.Account.ID == accountId).ToList();
                }

                if (accountCurrencyCode != mainCurrencyCode)
                {
                    wealthValue += allIncomesUntilRequestedPeriodInMainCurrency != null ? allIncomesUntilRequestedPeriodInMainCurrency.Sum(i => i.Summa) * rate : 0;
                }
                else
                {
                    wealthValue += allIncomesUntilRequestedPeriodInMainCurrency != null ? allIncomesUntilRequestedPeriodInMainCurrency.Sum(i => i.Summa) : 0;
                }

                List<Expenditure> allExpendituresUntilRequestedPeriodInMainCurrency = null;
                if (allExpenditresUntilRequestedPeriod != null && allExpenditresUntilRequestedPeriod.Any())
                {
                    allExpendituresUntilRequestedPeriodInMainCurrency = allExpenditresUntilRequestedPeriod.Where(i => i.Account.ID == accountId).ToList();
                }

                if (accountCurrencyCode != mainCurrencyCode)
                {
                    wealthValue -= allExpendituresUntilRequestedPeriodInMainCurrency != null ? allExpendituresUntilRequestedPeriodInMainCurrency.Sum(i => i.Summa) * rate : 0;
                }
                else
                {
                    wealthValue -= allExpendituresUntilRequestedPeriodInMainCurrency != null ? allExpendituresUntilRequestedPeriodInMainCurrency.Sum(i => i.Summa) : 0;
                }
            }
            );
            return wealthValue;
        }

    }
}