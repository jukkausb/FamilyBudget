using System.Globalization;

namespace FamilyBudget.v3.App_CodeBase.Widgets
{
    public class ExpenditureIncomeItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Period { get; set; }
        public string Expenditure { get { return ExpenditureValue.ToString(CultureInfo.CurrentUICulture); } }
        public string Income { get { return IncomeValue.ToString(CultureInfo.CurrentUICulture); } }
        public decimal ExpenditureValue { get; set; }
        public decimal IncomeValue { get; set; }
    }
}