using System;
using System.Collections.Generic;
using FamilyBudget.Www.App_CodeBase;

namespace FamilyBudget.Www.Models.Widgets
{
    public class MoneyWidgetModelBase : WidgetModelBase
    {
        public List<ExtendedSelectListItem> Accounts { get; set; }
        public string SelectedAccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}