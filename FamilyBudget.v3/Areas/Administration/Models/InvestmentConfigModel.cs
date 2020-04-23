using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class InvestmentConfigModel
    {
        public InvestmentInstrumentListModel InvestmentInstrumentListModel { get; set; }
    }

    public class InvestmentInstrumentListModel : BaseListModelSimple<InvestmentInstrument>
    {
    }
}