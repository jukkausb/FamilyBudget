using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class IncomeModel : BaseListModel<Income, IncomeFilterModel>
    {
        protected override void RestoreDefaultModelState()
        {
            SortDirection = "DESC";
            SortField = "Date";
        }
    }
}