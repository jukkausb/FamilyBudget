using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class ExpenditureModel : BaseListAwareModel<Expenditure, ExpenditureFilterModel>
    {
        public IEnumerable<ExtendedSelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        protected override void ParseCustomModelState(HttpRequestBase request)
        {
        }

        protected override void RestoreCustomModelState(string returnParameters)
        {
        }

        protected override void AddCustomRouteValues(IDictionary<string, object> dictionary)
        {
        }

        protected override string DecodeModelParametersCore()
        {
            return "";
        }
    }
}