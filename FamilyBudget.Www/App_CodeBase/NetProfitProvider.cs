using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Utils;
using FamilyBudget.Www.Models.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FamilyBudget.Www.App_CodeBase
{
    public class NetProfitProvider : INetProfitProvider
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IExpenditureRepository _expenditureRepository;

        public NetProfitProvider(IAccountRepository accountRepository, IIncomeRepository incomeRepository, IExpenditureRepository expenditureRepository)
        {
            _accountRepository = accountRepository;
            _incomeRepository = incomeRepository;
            _expenditureRepository = expenditureRepository;

            // Only CBRCurrencyProvider can get rates for periods in the past
            _currencyProvider = new CBRCurrencyProvider();
        }

        public decimal CalculateNetProfit(List<Account> accounts, List<Income> allIncomesInMonth, List<Expenditure> allExpenditresInMonth, string mainCurrencyCode)
        {
            decimal netProfitValue = 0;
            
            accounts.ForEach(a =>
            {
                var accountCurrencyCode = a.Currency.Code;
                var accountId = a.ID;

                decimal rate = accountCurrencyCode != mainCurrencyCode ? _currencyProvider.GetSellCurrencyRate(accountCurrencyCode, mainCurrencyCode) : 1;
                Logger.Info(string.Format("Exchange rate ({0}-{1}): {2}", a.Currency.Code, mainCurrencyCode, rate));


                List<Income> allIncomesInMonthInMainCurrency = null;
                if (allIncomesInMonth != null && allIncomesInMonth.Any())
                {
                    allIncomesInMonthInMainCurrency = allIncomesInMonth.Where(i => i.Account.ID == accountId).ToList();
                }

                if (accountCurrencyCode != mainCurrencyCode)
                {
                    netProfitValue += allIncomesInMonthInMainCurrency != null ? allIncomesInMonthInMainCurrency.Sum(i => i.Summa) * rate : 0;
                }
                else
                {
                    netProfitValue += allIncomesInMonthInMainCurrency != null ? allIncomesInMonthInMainCurrency.Sum(i => i.Summa) : 0;
                }


                List<Expenditure> allExpendituresInMonthInMainCurrency = null;
                if (allExpenditresInMonth != null && allExpenditresInMonth.Any())
                {
                    allExpendituresInMonthInMainCurrency = allExpenditresInMonth.Where(i => i.Account.ID == accountId).ToList();
                }

                if (accountCurrencyCode != mainCurrencyCode)
                {
                    netProfitValue -= allExpendituresInMonthInMainCurrency != null ? allExpendituresInMonthInMainCurrency.Sum(i => i.Summa) * rate : 0;
                }
                else
                {
                    netProfitValue -= allExpendituresInMonthInMainCurrency != null ? allExpendituresInMonthInMainCurrency.Sum(i => i.Summa) : 0;
                }
            }
            );
            return netProfitValue;
        }
    }
}