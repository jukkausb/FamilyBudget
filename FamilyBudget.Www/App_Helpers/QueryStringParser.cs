using System;
using System.Web;
using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.App_Helpers
{
    public static class QueryStringParser
    {
        public const string GridReturnParameters = "returnParams";
        public const string GridParametersEncodeFormatString = "{0}|{1};{2}|{3};{4}|{5}";
        public const string GridParametersDecodeFormatString = "?{0}={1}&{2}={3}&{4}={5}";
        public const string GridPageIndexParameterName = "page";
        public const string GridSortFieldParameterName = "sort";
        public const string GridSortDirectionParameterName = "sortdir";

        #region Parse parameters from Query string

        public static string ParseSortField(HttpRequestBase request)
        {
            if (request == null)
                return null;

            return request.QueryString[GridSortFieldParameterName];
        }

        public static int ParsePageIndex(HttpRequestBase request)
        {
            if (request == null)
                return 0;

            string pageIndexString = request.QueryString[GridPageIndexParameterName];

            if (string.IsNullOrEmpty(pageIndexString))
                return 0;

            return Int32.Parse(pageIndexString);
        }

        public static ListSortDirection ParseSortDirection(HttpRequestBase request)
        {
            if (request == null)
                return ListSortDirection.Ascending;

            string directionString = request.QueryString[GridSortDirectionParameterName];
            if (string.IsNullOrEmpty(directionString))
                return ListSortDirection.Ascending;

            if (directionString.ToLower() == "desc")
                return ListSortDirection.Descending;

            return ListSortDirection.Ascending;
        }

        #endregion

        #region Restore parameters

        public static string GetReturnParamsSection(string returnParams, string parameterName)
        {
            if (string.IsNullOrEmpty(returnParams) || string.IsNullOrEmpty(parameterName))
                return null;

            string[] parameterSections = returnParams.Split(';');

            foreach (string parameterSection in parameterSections)
            {
                if (parameterSection.StartsWith(parameterName))
                    return parameterSection;
            }

            return null;
        }

        public static int RestorePageIndex(string returnParams)
        {
            string parameterSection = GetReturnParamsSection(returnParams, GridPageIndexParameterName);
            string[] parameters = parameterSection.Split('|');

            string pageIndexString = parameters[1];
            if (string.IsNullOrEmpty(pageIndexString))
                return 0;

            int pageIndex = 0;
            int.TryParse(pageIndexString, out pageIndex);

            return pageIndex;
        }

        public static string RestoreSortField(string returnParams)
        {
            string parameterSection = GetReturnParamsSection(returnParams, GridSortFieldParameterName);
            string[] parameters = parameterSection.Split('|');

            return parameters[1];
        }

        public static ListSortDirection RestoreSortDirection(string returnParams)
        {
            string parameterSection = GetReturnParamsSection(returnParams, GridSortDirectionParameterName);
            string[] parameters = parameterSection.Split('|');

            string directionString = parameters[1];
            if (string.IsNullOrEmpty(directionString))
                return ListSortDirection.Ascending;

            int direction = 0;
            int.TryParse(directionString, out direction);

            if (direction == (int) ListSortDirection.Descending)
                return ListSortDirection.Descending;

            return ListSortDirection.Ascending;
        }

        #endregion

        public static string EncodeGridParameters(int pageIndex, string sortField, ListSortDirection sortDirection)
        {
            return string.Format(GridParametersEncodeFormatString,
                GridPageIndexParameterName, pageIndex,
                GridSortFieldParameterName, sortField,
                GridSortDirectionParameterName, (int) sortDirection);
        }

        public static string DecodeGridParameters(int pageIndex, string sortField, ListSortDirection sortDirection)
        {
            return string.Format(GridParametersDecodeFormatString,
                GridPageIndexParameterName, pageIndex,
                GridSortFieldParameterName, sortField,
                GridSortDirectionParameterName, sortDirection == ListSortDirection.Ascending ? "asc" : "desc");
        }
    }
}