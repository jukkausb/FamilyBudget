using FamilyBudget.Www.App_Utils;
using FileHelpers;
using System;
using System.Linq;
using System.Net;

namespace FamilyBudget.Www.App_CodeBase.Csv
{
    public class YahooCurrencyProvider : ICurrencyProvider
    {
        public void DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            TryGetCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);
        }

        public decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            YahooCurrencyRate[] rates = TryGetCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);

            if (rates != null && rates.Any())
            {
                return rates[0].SellRate;
            }
            return 0;
        }

        private YahooCurrencyRate[] TryGetCurrencyRatesInternal(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                var rates =
                    CacheHelper.Get<YahooCurrencyRate[]>(string.Format(YahooCurrencyRate.CurrencyRateCacheKeyFormat,
                        sellCurrencyCode, purchaseCurrencyCode));
                if (rates != null)
                {
                    return rates;
                }

                string csvData;
                using (var web = new WebClient())
                {
                    csvData =
                        web.DownloadString(string.Format(YahooCurrencyRate.RateSourceFormatString, sellCurrencyCode,
                            purchaseCurrencyCode));
                    CurrencyRateFileWriter.SaveRatesToFile(sellCurrencyCode, purchaseCurrencyCode, csvData);
                }
                var engine = new FileHelperEngine<YahooCurrencyRate>();
                rates = engine.ReadString(csvData);
                CacheHelper.Add(rates,
                    string.Format(YahooCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                GlobalExceptionHandler.RemoveApplicationWarning();
                return rates;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют (YahooCurrencyProvider)", ex);
                return null;
            }
        }
    }
}