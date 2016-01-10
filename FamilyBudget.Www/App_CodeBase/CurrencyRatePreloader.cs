
using System;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Repository.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FamilyBudget.Www.App_CodeBase
{
    public static class CurrencyRatePreloader
    {
        private static IAccountRepository _accountRepository;
        private static ICurrencyProvider _currencyProvider;

        public static void Preload()
        {
            _accountRepository = ServiceLocator.Current.GetInstance<IAccountRepository>();
            _currencyProvider = ServiceLocator.Current.GetInstance<ICurrencyProvider>();

            Account mainAccount = _accountRepository.GetAll().FirstOrDefault(a => a.IsMain);
            if (mainAccount == null)
                throw new Exception("Unable to find main account");

            var currencyCodesFrom = _accountRepository.GetAll().Select(a => a.Currency.Code);
            string mainCurrency = mainAccount.Currency.Code;
            foreach (string currencyCodeFrom in currencyCodesFrom)
            {
                if (currencyCodeFrom != mainCurrency)
                {
                    _currencyProvider.DownloadCurrencyRates(currencyCodeFrom, mainCurrency);
                }
            }
        }
    }
}