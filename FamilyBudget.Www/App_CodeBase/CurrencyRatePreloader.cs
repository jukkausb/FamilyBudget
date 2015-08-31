
using System;
using System.Linq;
using Antlr.Runtime;
using FamilyBudget.Www.App_CodeBase.Csv;
using FamilyBudget.Www.Repository.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace FamilyBudget.Www.App_CodeBase
{
    public static class CurrencyRatePreloader
    {
        private static IAccountRepository _accountRepository;

        public static void Preload()
        {
            _accountRepository = ServiceLocator.Current.GetInstance<IAccountRepository>();
            var currencyCodes = _accountRepository.GetAll().Select(a => a.Currency.Code);
            var currencyCodes2 = _accountRepository.GetAll().Select(a => a.Currency.Code);

            foreach (string currencyCode in currencyCodes)
            {
                foreach (var currencyCode2 in currencyCodes2)
                {
                    if (currencyCode != currencyCode2)
                    {
                        CurrencyProvider.DownloadCurrencyRates(currencyCode, currencyCode2);
                    }
                }
            }
        }
    }
}