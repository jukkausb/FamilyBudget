using System.Collections.Generic;
using FamilyBudget.v3.Models.Home;

namespace FamilyBudget.v3.Models.Widgets
{
    public class AccountBalanceWidgetModel : WidgetModelBase
    {
        public string MainCurrency { get; set; }
        public List<AccountRateView> Accounts { get; set; }
        public MoneyModel Wealth { get; set; }
    }
}