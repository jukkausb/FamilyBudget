using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class ExpenditureListModel : BaseListModel<Expenditure, ExpenditureFilterModel>
    {
        public ExpenditureListModel()
        {
            Filter = new ExpenditureFilterModel();
        }
    }
}