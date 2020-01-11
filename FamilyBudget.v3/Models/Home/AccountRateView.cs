namespace FamilyBudget.v3.Models.Home
{
    public class AccountRateView
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Equivalent { get { return Balance * Rate; } }
        public decimal Rate { get; set; }
        public decimal Percent { get; set; }
    }
}