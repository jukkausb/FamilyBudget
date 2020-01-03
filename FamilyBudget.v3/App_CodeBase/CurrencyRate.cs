
using System;

namespace FamilyBudget.v3.App_CodeBase
{
    public class CurrencyRate
    {
        public const string CurrencyRateCacheKeyFormat = "CurrencyRateCacheKey_{0}_{1}";

        public string FromValuta { get; set; }
        public string ToValuta { get; set; }
        public decimal Rate { get; set; }
        public decimal Nominal { get; set; }
        public DateTime Date { get; set; }
    }
}