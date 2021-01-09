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
        public void DownloadCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            TryGetCurrencyRateInternal(sellCurrencyCode, purchaseCurrencyCode);
        }

        public double GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            FreeCurrencyRate rate = TryGetCurrencyRateInternal(sellCurrencyCode, purchaseCurrencyCode);
            return rate?.Val ?? 0;
        }

        public void SetCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode, double value)
        {
            string key = string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode);
            var rate = CacheHelper.Get<FreeCurrencyRate>(key);
            if (rate == null)
            {
                return;
            }

            rate.Val = value;

            CacheHelper.Clear(key);
            CacheHelper.Add(rate, key);
        }

        private FreeCurrencyRate TryGetCurrencyRateInternal(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            var rate = CacheHelper.Get<FreeCurrencyRate>(string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
            if (rate != null)
            {
                return rate;
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

                rate = JsonConvert.DeserializeObject<Dictionary<string, double>>(jsonData).Select(x => new FreeCurrencyRate { Val = x.Value }).FirstOrDefault();

                CacheHelper.Add(rate, string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                GlobalExceptionHandler.RemoveApplicationWarning();
                return rate;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют (FreeCurrencyConverter)", ex);
                Logger.Error("Попытка запроса курсов валют из последних файлов ... (FreeCurrencyConverter)", ex);

                try
                {
                    string jsonData = CurrencyRateFileWriter.ReadRatesFromJsonFile(sellCurrencyCode, purchaseCurrencyCode);
                    rate = JsonConvert.DeserializeObject<Dictionary<string, double>>(jsonData).Select(x => new FreeCurrencyRate { Val = x.Value }).FirstOrDefault();
                    CacheHelper.Add(rate, string.Format(FreeCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
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