using FamilyBudget.Www.App_DataModel;

namespace FamilyBudget.Www.Models.Home
{
    public class AccountRateView
    {
        public Account Account { get; set; }
        public CurrencyRateView RateView { get; set; }
    }
}