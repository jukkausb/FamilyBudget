using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
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

        public void InitializeFilter(IEnumerable<SelectListItem> accounts)
        {
            Filter.Accounts = accounts;
        }

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