using System.Collections.Generic;
using FamilyBudget.v3.Models.Home;

namespace FamilyBudget.v3.Models.Widgets
{
    public class AccountBalanceCircleWidgetModel : WidgetModelBase
    {
        public List<AccountCircleEquivalentView> Accounts { get; set; }
    }
}