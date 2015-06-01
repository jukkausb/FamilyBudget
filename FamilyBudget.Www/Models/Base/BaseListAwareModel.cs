using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FamilyBudget.Www.App_Helpers;

namespace FamilyBudget.Www.Models.Base
{
    public abstract class BaseListAwareModel<T, TK> : BaseModel where TK : BaseFilterModel
    {
        public T Object { get; set; }
        public TK Filter { get; set; }
        public int PageIndex { get; set; }
        public string SortField { get; set; }
        public ListSortDirection SortDirection { get; set; }

        public void ParseModelState(HttpRequestBase request)
        {
            PageIndex = QueryStringParser.ParsePageIndex(request);
            SortDirection = QueryStringParser.ParseSortDirection(request);
            SortField = QueryStringParser.ParseSortField(request);
            ParseCustomModelState(request);
        }

        protected virtual void ParseCustomModelState(HttpRequestBase request)
        {
        }

        protected virtual void RestoreCustomModelState(string returnParameters)
        {
        }

        protected virtual void AddCustomRouteValues(IDictionary<string, object> dictionary)
        {
        }

        protected virtual string DecodeModelParametersCore()
        {
            return string.Empty;
        }

        public void RestoreModelState(string returnParameters)
        {
            PageIndex = QueryStringParser.RestorePageIndex(returnParameters);
            SortDirection = QueryStringParser.RestoreSortDirection(returnParameters);
            SortField = QueryStringParser.RestoreSortField(returnParameters);
            RestoreCustomModelState(returnParameters);
        }

        public RouteValueDictionary ToRouteValueDictionary()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add(QueryStringParser.GridPageIndexParameterName, PageIndex);
            dict.Add(QueryStringParser.GridSortFieldParameterName, SortField);
            dict.Add(QueryStringParser.GridSortDirectionParameterName,
                SortDirection == ListSortDirection.Ascending ? "asc" : "desc");

            AddCustomRouteValues(dict);

            return new RouteValueDictionary(dict);
        }

        public string DecodeModelParameters()
        {
            string coreParameters = DecodeModelParametersCore();
            return string.Format("{0}&{1}", QueryStringParser.DecodeGridParameters(PageIndex, SortField, SortDirection),
                coreParameters);
        }
    }
}