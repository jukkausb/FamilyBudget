using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Base;
using FamilyBudget.v3.Models.Filter;

namespace FamilyBudget.v3.Models
{
    public class ExpenditureModel : BaseListModel<Expenditure, ExpenditureFilterModel>
    {
        protected override void RestoreDefaultModelState()
        {
            SortDirection = "DESC";
            SortField = "Date";
        }
    }
}