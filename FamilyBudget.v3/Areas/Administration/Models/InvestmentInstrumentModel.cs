using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Base;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class InvestmentInstrumentModel : BaseListAwareModel<InvestmentInstrument>
    {
        public IEnumerable<SelectListItem> InvestmentInstrumentTypes { get; set; }
        public IEnumerable<SelectListItem> InvestmentInstrumentMarkets { get; set; }

        public override FormActionButtonsModel FormActionButtons => new FormActionButtonsModel
        {
            BackButtonLink = "/Administration/InvestmentInstrument" + QueryStringParser.DecodeGridParameters(PageIndex, SortField, SortDirection),
            RenderBackButton = true,
            RenderSaveButton = true,
            RenderSaveAndCreateButton = true
        };
    }
}