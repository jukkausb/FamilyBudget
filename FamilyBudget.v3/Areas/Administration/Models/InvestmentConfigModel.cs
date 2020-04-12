using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class InvestmentConfigModel
    {
        public InvestmentRulesEtfListModel InvestmentRulesEtfListModel { get; set; }
        public InvestmentRulesInstrumentsListModel InvestmentRulesInstrumentsListModel { get; set; }
    }

    public class InvestmentRulesEtfListModel : BaseListModelSimple<InvestmentRulesEtf>
    {
    }

    public class InvestmentRulesInstrumentsListModel : BaseListModelSimple<InvestmentRulesInstruments>
    {
    }
}