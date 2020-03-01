using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class ExpenditureCategoryModel : BaseListAwareModel<ExpenditureCategory>
    {
        public override FormActionButtonsModel FormActionButtons => new FormActionButtonsModel
        {
            BackButtonLink = "/Administration/ExpenditureCategory" + QueryStringParser.DecodeGridParameters(PageIndex, SortField, SortDirection),
            RenderBackButton = true,
            RenderSaveButton = true,
            RenderSaveAndCreateButton = true
        };
    }
}