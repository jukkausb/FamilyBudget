
namespace FamilyBudget.Www.Models.Home
{
    public class CurrencyRateView
    {
        public decimal SellRate { get; set; }
        public string OriginCurrency { get; set; }
        public string MainCurrency { get; set; }
        public decimal Equivalent { get; set; }
    }
}