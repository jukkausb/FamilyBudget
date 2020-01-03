using System.Linq;
using FamilyBudget.v3.App_Utils;
using FileHelpers;
using System;
using System.Net;

namespace FamilyBudget.v3.App_CodeBase.Csv
{
    public class GoldOunceRateProvider : IGoldOunceRateProvider
    {
        public void DownloadGoldOunceRates(string currency)
        {
            TryGetGoldOunceRatesInternal(currency);
        }

        public decimal GetAverageGoldOunceRate(string currency, int year, int month)
        {
            GoldOunceRate[] rates = TryGetGoldOunceRatesInternal(currency);

            if (rates != null && rates.Any())
            {
                var ratesForPeriod = rates.Where(r => r.Date.Year == year && r.Date.Month == month).ToList();
                if (ratesForPeriod.Any())
                {
                    return ratesForPeriod.Average(r => r.Rate);
                }
            }

            return 0;
        }

        private GoldOunceRate[] TryGetGoldOunceRatesInternal(string currency)
        {
            try
            {
                var rates =
                    CacheHelper.Get<GoldOunceRate[]>(string.Format(GoldOunceRate.CurrencyRateCacheKeyFormat, currency));
                if (rates != null)
                {
                    return rates;
                }

                string csvData;
                using (var web = new WebClient())
                {
                    csvData = web.DownloadString(GoldOunceRate.RateSourceString);
                    GoldOunceRateFileWriter.SaveRatesToFile(currency, csvData);
                }
                var engine = new FileHelperEngine<GoldOunceRate>();
                rates = engine.ReadString(csvData);
                CacheHelper.Add(rates, string.Format(GoldOunceRate.CurrencyRateCacheKeyFormat, currency));
                GlobalExceptionHandler.RemoveApplicationWarning();
                return rates;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса стоимости унции золота (GoldOunceRateProvider)", ex);
                return null;
            }
        }
    }
}