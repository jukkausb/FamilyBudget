using FamilyBudget.v3.App_CodeBase.Csv;
using FamilyBudget.v3.App_Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FamilyBudget.v3.App_CodeBase.Json
{
    public class FreeCurrencyConverter : ICurrencyProvider
    {
        public void DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            TryGetCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);
        }

        public decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            FreeCurrencyRate[] rates = TryGetCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);

            if (rates != null && rates.Any())
            {
                return rates[0].Val;
            }
            return 0;
        }

        private FreeCurrencyRate[] TryGetCurrencyRatesInternal(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            var rates = CacheHelper.Get<FreeCurrencyRate[]>(string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
            if (rates != null)
            {
                return rates;
            }

            try
            {
                string jsonData;
                using (var web = new WebClient())
                {
                    jsonData = web.DownloadString(string.Format(FreeCurrencyRate.RateSourceFormatString, sellCurrencyCode,
                            purchaseCurrencyCode));
                    CurrencyRateFileWriter.SaveRatesToJsonFile(sellCurrencyCode, purchaseCurrencyCode, jsonData);
                }

                rates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jsonData).Select(x => new FreeCurrencyRate() { Val = x.Value }).ToArray();

                CacheHelper.Add(rates, string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                GlobalExceptionHandler.RemoveApplicationWarning();
                return rates;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют (FreeCurrencyConverter)", ex);
                Logger.Error("Попытка запроса курсов валют из последних файлов ... (FreeCurrencyConverter)", ex);

                try
                {
                    string jsonData = CurrencyRateFileWriter.ReadRatesFromJsonFile(sellCurrencyCode, purchaseCurrencyCode);
                    rates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jsonData).Select(x => new FreeCurrencyRate() { Val = x.Value }).ToArray();
                    CacheHelper.Add(rates, string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                    GlobalExceptionHandler.RemoveApplicationWarning();
                }
                catch (Exception exFiles)
                {
                    Logger.Error("Ошибка запроса курсов валют из последних файлов ... (FreeCurrencyConverter)", exFiles);
                    return null;
                }

                return null;
            }
        }
    }
}