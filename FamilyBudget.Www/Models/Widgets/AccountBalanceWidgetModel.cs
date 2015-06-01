using System.Collections.Generic;
using FamilyBudget.Www.Models.Home;

namespace FamilyBudget.Www.Models.Widgets
{
    public class AccountBalanceWidgetModel : WidgetModelBase
    {
        public string MainCurrency { get; set; }
        public List<AccountRateView> Accounts { get; set; }
        public WealthModel Wealth { get; set; }
    }
}