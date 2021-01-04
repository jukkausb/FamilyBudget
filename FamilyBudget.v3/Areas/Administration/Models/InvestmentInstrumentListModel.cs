using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Base;
using System.Collections.Generic;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class InvestmentInstrumentListModel : BaseListAwareModel<InvestmentInstrumentInfoModel>
    {
        public List<MessageGroup> MessageGroups { get; set; }

        public InvestmentInstrumentListModel()
        {
            MessageGroups = new List<MessageGroup>();
        }
    }
}