using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Spa;

namespace FamilyBudget.v3.Models.Home
{
    public class AccountRateView
    {
        public Account Account { get; set; }
        public AccountModel AccountModel { get; set; }
        public CurrencyRateView RateView { get; set; }
    }

    public class AccountRateViewNew
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Equivalent { get { return Balance * Rate; } }
        public decimal Rate { get; set; }
    }
}