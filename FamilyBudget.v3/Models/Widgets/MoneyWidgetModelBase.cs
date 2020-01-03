using System;

namespace FamilyBudget.v3.Models.Widgets
{
    public class MoneyWidgetModelBase : WidgetModelBase
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}