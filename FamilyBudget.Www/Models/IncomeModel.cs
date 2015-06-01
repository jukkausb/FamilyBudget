using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class IncomeModel : BaseListAwareModel<Income, IncomeFilterModel>
    {
        public IEnumerable<ExtendedSelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        protected override void ParseCustomModelState(HttpRequestBase request)
        {
            if (Filter == null)
            {
                Filter = new IncomeFilterModel();
            }

            Filter.Description = IncomeListModel.ParseFilterDescriptionField(request);
        }

        protected override void RestoreCustomModelState(string returnParameters)
        {
            if (Filter == null)
            {
                Filter = new IncomeFilterModel();
            }

            Filter.Description = IncomeListModel.RestoreFilterField(returnParameters,
                IncomeListModel.GridFilterDescriptionFieldParameterName);

            int accountId;
            int.TryParse(
                IncomeListModel.RestoreFilterField(returnParameters,
                    IncomeListModel.GridFilterAccountIdFieldParameterName), out accountId);
            Filter.AccountId = accountId;
        }

        protected override string DecodeModelParametersCore()
        {
            return string.Format("{0}={1}&{2}={3}", IncomeListModel.GridFilterDescriptionFieldParameterName,
                Filter.Description,
                IncomeListModel.GridFilterAccountIdFieldParameterName, Filter.AccountId);
        }

        protected override void AddCustomRouteValues(IDictionary<string, object> dictionary)
        {
            dictionary.Add(IncomeListModel.GridFilterDescriptionFieldParameterName, Filter.Description);
            dictionary.Add(IncomeListModel.GridFilterAccountIdFieldParameterName, Filter.AccountId);
        }
    }
}