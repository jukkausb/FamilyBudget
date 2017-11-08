﻿namespace FamilyBudget.Www.App_CodeBase.Json
{
    public class FreeCurrencyRate
    {
        public const string RateSourceFormatString = "http://free.currencyconverterapi.com/api/v4/convert?q={0}_{1}&compact=ultra";
        public const string CurrencyRateCacheKeyFormat = "FreeCurrencyRateCacheKey_{0}_{1}";

        public decimal Val { get; set; }
    }
}