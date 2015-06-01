using System.Collections.Generic;
using FamilyBudget.Www.Models.Home;

namespace FamilyBudget.Www.Models.Widgets
{
    public class AccountBalanceCircleWidgetModel : WidgetModelBase
    {
        public List<AccountCircleEquivalentView> Accounts { get; set; }
    }
}