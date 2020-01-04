namespace FamilyBudget.v3.Models.Home
{
    public class AccountRateViewNew
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Equivalent { get { return Balance * Rate; } }
        public decimal Rate { get; set; }
    }
}