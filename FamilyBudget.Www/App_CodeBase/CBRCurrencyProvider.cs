using FamilyBudget.Www.App_Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FamilyBudget.Www.App_CodeBase
{
    public class CBRCurrencyProvider : ICurrencyProvider
    {
        private static readonly DateTime StartDateTime = new DateTime(2010, 1, 1);

        public void DownloadCurrencyRates(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            TryGetCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);
        }

        public decimal GetSellCurrencyRate(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            CurrencyRate[] rates = TryGetCurrencyRatesInternal(sellCurrencyCode, purchaseCurrencyCode);

            if (rates != null && rates.Any())
            {
                var rate = rates.Last();
                return rate.Rate / rate.Nominal;
            }
            return 0;
        }

        private CurrencyRate[] TryGetCurrencyRatesInternal(string sellCurrencyCode, string purchaseCurrencyCode)
        {
            if (purchaseCurrencyCode != "RUB")
                throw new Exception("CBRCurrencyProvider does not support convertion rates other than to RUB");

            try
            {
                var client = new CBRService.DailyInfoSoapClient();

                var valutes = CacheHelper.Get<List<Valuta>>(Valuta.ValutaListCacheKey);
                if (valutes == null)
                {
                    var valutesRows = client.EnumValutes(false).Tables["EnumValutes"].AsEnumerable().ToList();
                    valutes = (from valutesRow in valutesRows
                               let nominalValue = GetValue<decimal>(valutesRow, "Vnom")
                               select new Valuta
                               {
                                   VEngname = GetText(valutesRow, "VEngname"),
                                   VcharCode = GetText(valutesRow, "VcharCode"),
                                   Vcode = GetText(valutesRow, "Vcode"),
                                   VcommonCode = GetText(valutesRow, "VcommonCode"),
                                   Vname = GetText(valutesRow, "Vname"),
                               }).ToList();

                    CacheHelper.Add(valutes, Valuta.ValutaListCacheKey);
                }

                var sellValuta = valutes.FirstOrDefault(v => v.VcharCode == sellCurrencyCode);
                if (sellValuta == null)
                    throw new Exception("CBRCurrencyProvider cannot find valuta with code = " + sellCurrencyCode);

                var rates = CacheHelper.Get<CurrencyRate[]>(string.Format(CurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                if (rates != null)
                    return rates;

                rates = client.GetCursDynamic(StartDateTime, DateTime.Now, sellValuta.Vcode).Tables["ValuteCursDynamic"].AsEnumerable().Select(row =>
                new CurrencyRate
                {
                    FromValuta = sellCurrencyCode,
                    ToValuta = purchaseCurrencyCode,
                    Rate = row.Field<decimal>("Vcurs"),
                    Nominal = row.Field<decimal>("Vnom"),
                }).ToArray();

                CacheHelper.Add(rates, string.Format(CurrencyRate.CurrencyRateCacheKeyFormat, sellCurrencyCode, purchaseCurrencyCode));
                GlobalExceptionHandler.RemoveApplicationWarning();
                return rates;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.SetApplicationWarning(ex);
                Logger.Error("Ошибка запроса курсов валют (CBRCurrencyProvider)", ex);
                return null;
            }
        }

        private static T? GetValue<T>(DataRow row, string columnName) where T : struct
        {
            if (row.IsNull(columnName))
                return null;

            return row[columnName] as T?;
        }

        private static string GetText(DataRow row, string columnName)
        {
            if (row.IsNull(columnName))
                return string.Empty;

            return row[columnName] as string ?? string.Empty;
        }
    }
}