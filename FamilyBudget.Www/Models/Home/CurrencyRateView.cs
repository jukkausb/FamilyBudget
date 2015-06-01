using FamilyBudget.Www.App_CodeBase.Csv;

namespace FamilyBudget.Www.Models.Home
{
    public class CurrencyRateView
    {
        public CurrencyRate Rate { get; set; }
        public string OriginCurrency { get; set; }
        public string MainCurrency { get; set; }
        public decimal Equivalent { get; set; }
    }
}