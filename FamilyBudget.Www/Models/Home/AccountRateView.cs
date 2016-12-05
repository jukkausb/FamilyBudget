using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Spa;

namespace FamilyBudget.Www.Models.Home
{
    public class AccountRateView
    {
        public Account Account { get; set; }
        public AccountModel AccountModel { get; set; }
        public CurrencyRateView RateView { get; set; }
    }
}