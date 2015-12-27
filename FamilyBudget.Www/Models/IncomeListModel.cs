using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class IncomeListModel : BaseListModel<Income, IncomeFilterModel>
    {
        public IncomeListModel()
        {
            Filter = new IncomeFilterModel();
        }
    }
}