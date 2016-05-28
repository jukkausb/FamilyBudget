
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Repository.Interfaces;
using System;
using System.Linq;

namespace FamilyBudget.Www.App_CodeBase
{
    public class CurrencyRatePreloader
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrencyProvider _currencyProvider;

        public CurrencyRatePreloader(IAccountRepository accountRepository, ICurrencyProvider currencyProvider)
        {
            _accountRepository = accountRepository;
            _currencyProvider = currencyProvider;
        }

        public void Preload()
        {
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