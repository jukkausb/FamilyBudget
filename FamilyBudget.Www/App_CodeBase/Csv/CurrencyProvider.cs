using System;
using System.Linq;
using System.Net;
using FamilyBudget.Www.App_Utils;
using FileHelpers;
using System.IO;

namespace FamilyBudget.Www.App_CodeBase.Csv
{
    public static class CurrencyProvider
    {
        private static CurrencyRate[] DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode)
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
                Logger.Error("Ошибка запроса курсов валют", ex);
                return null;
            }
        }

        public static CurrencyRate GetCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                CurrencyRate[] rates = DownloadCurrencyRates(sellCurrencyCode, purchaseCurrencyCode);

                // Try get rates from latest successful session
                if (rates == null)
                {
                    var engine = new FileHelperEngine<CurrencyRate>();
                    rates = engine.ReadString(CurrencyRateFileWriter.ReadRatesFromFile(sellCurrencyCode, purchaseCurrencyCode));
                }

                if (rates == null)
                    return null;

                return
                    rates.FirstOrDefault(
                        r => r.ConversionPath.Contains(string.Format("{0}{1}", sellCurrencyCode, purchaseCurrencyCode)));
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка запроса курсов валют", ex);
                return null;
            }
        }

        public static decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                CurrencyRate[] rates = DownloadCurrencyRates(sellCurrencyCode, purchaseCurrencyCode);
                if (rates != null && rates.Any())
                {
                    return rates[0].SellRate;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка запроса курсов валют", ex);
                return 0;
            }
        }

        public static decimal GetPurchaseCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                CurrencyRate[] rates = DownloadCurrencyRates(sellCurrencyCode, purchaseCurrencyCode);
                if (rates != null && rates.Any())
                {
                    return rates[0].PurchaseRate;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка запроса курсов валют", ex);
                return 0;
            }
        }

        public static decimal GetMainCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            try
            {
                CurrencyRate[] rates = DownloadCurrencyRates(sellCurrencyCode, purchaseCurrencyCode);
                if (rates != null && rates.Any())
                {
                    return rates[0].MainRate;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка запроса курсов валют", ex);
                return 0;
            }
        }
    }
}