
using FamilyBudget.Www.App_CodeBase.Csv;
using FamilyBudget.Www.Repository.Interfaces;
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

            var currencyCodesFrom = _accountRepository.GetAll().Select(a => a.Currency.Code);
            var currencyCodesTo = _accountRepository.GetAll().Select(a => a.Currency.Code);

            foreach (string currencyCodeFrom in currencyCodesFrom)
            {
                foreach (var currencyCodeTo in currencyCodesTo)
                {
                    if (currencyCodeFrom != currencyCodeTo)
                    {
                        _currencyProvider.DownloadCurrencyRates(currencyCodeFrom, currencyCodeTo);
                    }
                }
            }
        }
    }
}