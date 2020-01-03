using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_Helpers;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace FamilyBudget.v3.Models.Base
{
    public abstract class BaseListModel<T, TK> : BaseListAwareModel<T> where TK : BaseMoneyFilterModel, new()
    {
        public const string GridFilterDescriptionFieldParameterName = "Filter.Description";
        public const string GridFilterAccountIdFieldParameterName = "Filter.AccountId";

        public List<string> DescriptionSuggestions { get; set; }

        public TK Filter { get; set; }

        public IEnumerable<ExtendedSelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        protected BaseListModel()
        {
            Filter = new TK();
            PageSize = 20;
        }

        public string EncodeModelParameters()
        {
            string coreParameters = EncodeModelParametersCore();
            return string.Format("{0};{1}", QueryStringParser.EncodeGridParameters(PageIndex, SortField, SortDirection),
                coreParameters);
        }

        public void InitializeFilter(IEnumerable<SelectListItem> accounts, IEnumerable<SelectListItem> categories)
        {
            Filter.Accounts = accounts;
            Filter.Categories = categories;
        }

        public static string ParseFilterDescriptionField(HttpRequestBase request)
        {
            if (request == null)
                return null;

            return request.QueryString[GridFilterDescriptionFieldParameterName];
        }

        public static string ParseFilterAccountIdField(HttpRequestBase request)
        {
            if (request == null)
                return null;

            return request.QueryString[GridFilterAccountIdFieldParameterName];
        }

        protected override void ParseCustomModelState(HttpRequestBase request)
        {
            ParseFilterDescriptionField(request);
            ParseFilterAccountIdField(request);
        }

        protected override void RestoreCustomModelState(string returnParameters)
        {
            Filter.Description = RestoreFilterField(returnParameters, GridFilterDescriptionFieldParameterName);

            int accountId;
            int.TryParse(RestoreFilterField(returnParameters, GridFilterAccountIdFieldParameterName), out accountId);
            Filter.AccountId = accountId;
        }

        public static string RestoreFilterField(string returnParams, string parameterFieldName)
        {
            string parameterSection = QueryStringParser.GetReturnParamsSection(returnParams, parameterFieldName);
            string[] parameters = parameterSection.Split('|');
            return parameters[1];
        }

        protected string EncodeModelParametersCore()
        {
            return string.Format("{0}|{1};{2}|{3}", GridFilterDescriptionFieldParameterName, Filter.Description,
                GridFilterAccountIdFieldParameterName, Filter.AccountId);
        }

        protected override string DecodeModelParametersCore()
        {
            return string.Format("{0}={1}&{2}={3}", GridFilterDescriptionFieldParameterName,
                Filter.Description,
                GridFilterAccountIdFieldParameterName, Filter.AccountId);
        }

        protected override void AddCustomRouteValues(IDictionary<string, object> dictionary)
        {
            dictionary.Add(GridFilterDescriptionFieldParameterName, Filter.Description);
            dictionary.Add(GridFilterAccountIdFieldParameterName, Filter.AccountId);
        }
    }
}