using FamilyBudget.v3.Models.Home;
using System.Collections.Generic;

namespace FamilyBudget.v3.Models.Widgets
{
    public class BalanceAndInvestmentCircleWidgetModel : WidgetModelBase
    {
        public List<AccountRateView> AccountRateViews { get; set; }
    }
}