using System;
using System.Linq;
using System.Net;
using FamilyBudget.Www.App_Utils;
using FileHelpers;

namespace FamilyBudget.Www.App_CodeBase.Csv
{
    public class CurrencyProvider : ICurrencyProvider
    {
        public CurrencyRate[] DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            return DownloadCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);
        }

        public CurrencyRate GetCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                CurrencyRate[] rates = TryGetCurrencyRates(sellCurrencyCode, purchaseCurrencyCode);

                if (rates == null)
                    return null;

                return
                    rates.FirstOrDefault(
                        r => r.ConversionPath.Contains(string.Format("{0}{1}", sellCurrencyCode, purchaseCurrencyCode)));
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют", ex);
                return null;
            }
        }

        public decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                CurrencyRate[] rates = TryGetCurrencyRates(sellCurrencyCode, purchaseCurrencyCode);

                if (rates != null && rates.Any())
                {
                    return rates[0].SellRate;
                }
                return 0;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют", ex);
                return 0;
            }
        }

        private CurrencyRate[] DownloadCurrencyRatesInternal(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                var rates =
                    CacheHelper.Get<CurrencyRate[]>(string.Format(CurrencyRate.CurrencyRateCacheKeyFormat,
                        sellCurrencyCode, purchaseCurrencyCode));
                if (rates != null)
                {
                    return rates;
                }

                string csvData;
                using (var web = new WebClient())
                {
                    csvData =
                        web.DownloadString(string.Format(CurrencyRate.RateSourceFormatString, sellCurrencyCode,
                            purchaseCurrencyCode));
                    CurrencyRateFileWriter.SaveRatesToFile(sellCurrencyCode, purchaseCurrencyCode, csvData);
                }
                var engine = new FileHelperEngine<CurrencyRate>();
                rates = engine.ReadString(csvData);
                CacheHelper.Add(rates,
                    string.Format(CurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                return rates;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют", ex);
                return null;
            }
        }

        private CurrencyRate[] TryGetCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            CurrencyRate[] rates = DownloadCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);

            // Try get rates from latest successful session
            if (rates == null)
            {
                var engine = new FileHelperEngine<CurrencyRate>();
                rates = engine.ReadString(CurrencyRateFileWriter.ReadRatesFromFile(sellCurrencyCode, purchaseCurrencyCode));
            }
            return rates;
        }
    }
}