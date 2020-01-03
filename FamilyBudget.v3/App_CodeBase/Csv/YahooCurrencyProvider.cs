using FamilyBudget.v3.App_Utils;
using FileHelpers;
using System;
using System.Linq;
using System.Net;

namespace FamilyBudget.v3.App_CodeBase.Csv
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
            var engine = new FileHelperEngine<YahooCurrencyRate>();
            var rates = CacheHelper.Get<YahooCurrencyRate[]>(string.Format(YahooCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
            if (rates != null)
            {
                return rates;
            }

            try
            {
                string csvData;
                using (var web = new WebClient())
                {
                    csvData =
                        web.DownloadString(string.Format(YahooCurrencyRate.RateSourceFormatString, sellCurrencyCode,
                            purchaseCurrencyCode));
                    CurrencyRateFileWriter.SaveRatesToCsvFile(sellCurrencyCode, purchaseCurrencyCode, csvData);
                }

                rates = engine.ReadString(csvData);
                CacheHelper.Add(rates, string.Format(YahooCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                GlobalExceptionHandler.RemoveApplicationWarning();
                return rates;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют (YahooCurrencyProvider)", ex);
                Logger.Error("Попытка запроса курсов валют из последних файлов ... (YahooCurrencyProvider)", ex);

                try
                {
                    string csvData = CurrencyRateFileWriter.ReadRatesFromCsvFile(sellCurrencyCode, purchaseCurrencyCode);
                    rates = engine.ReadString(csvData);
                    CacheHelper.Add(rates, string.Format(YahooCurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                    GlobalExceptionHandler.RemoveApplicationWarning();
                }
                catch (Exception exFiles)
                {
                    Logger.Error("Ошибка запроса курсов валют из последних файлов ... (YahooCurrencyProvider)", exFiles);
                    return null;
                }

                return null;
            }
        }

        public decimal GetSellCurrencyRateAverageForPeriod(string sellCurrencyCode, string purchaseCurrencyCode, int month, int year)
        {
            throw new NotSupportedException();
        }
    }
}