using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Models.Base;
using FamilyBudget.Www.Models.Filter;

namespace FamilyBudget.Www.Models
{
    public class IncomeListModel : BaseListModel<Income, IncomeFilterModel>
    {
        public const string GridFilterDescriptionFieldParameterName = "Filter.Description";
        public const string GridFilterAccountIdFieldParameterName = "Filter.AccountId";

        public IncomeListModel()
        {
            Filter = new IncomeFilterModel();
        }

        public void InitializeFilter(IEnumerable<SelectListItem> accounts)
        {
            Filter.Accounts = accounts;
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

            int accountId = 0;
            int.TryParse(RestoreFilterField(returnParameters, GridFilterAccountIdFieldParameterName), out accountId);
            Filter.AccountId = accountId;
        }

        public static string RestoreFilterField(string returnParams, string parameterFieldName)
        {
            string parameterSection = QueryStringParser.GetReturnParamsSection(returnParams, parameterFieldName);
            string[] parameters = parameterSection.Split('|');
            return parameters[1];
        }

        protected override string EncodeModelParametersCore()
        {
            return string.Format("{0}|{1};{2}|{3}", GridFilterDescriptionFieldParameterName, Filter.Description,
                GridFilterAccountIdFieldParameterName, Filter.AccountId);
        }

        protected override void AddCustomRouteValues(IDictionary<string, object> dictionary)
        {
            throw new NotImplementedException();
        }

        protected override string DecodeModelParametersCore()
        {
            throw new NotImplementedException();
        }
    }
}