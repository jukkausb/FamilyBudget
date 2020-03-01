using FamilyBudget.v3.App_Helpers;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace FamilyBudget.v3.Models.Base
{
    public abstract class BaseListAwareModel<T> : BaseModel
    {
        public T Object { get; set; }
        public virtual FormActionButtonsModel FormActionButtons { get; }
        public int PageIndex { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        
        /// <summary>
        ///     Page size of the grid displaying list of entities
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Entity collection
        /// </summary>
        public List<T> Entities { get; set; }

        protected BaseListAwareModel()
        {
            PageSize = 10;
        }

        public void ParseModelState(HttpRequestBase request)
        {
            PageIndex = QueryStringParser.ParsePageIndex(request);
            SortDirection = QueryStringParser.ParseSortDirection(request);
            SortField = QueryStringParser.ParseSortField(request);
            ParseCustomModelState(request);
        }

        public void RestoreModelState(string returnParameters)
        {
            if (string.IsNullOrEmpty(returnParameters))
            {
                RestoreDefaultModelState();
                return;
            }

            PageIndex = QueryStringParser.RestorePageIndex(returnParameters);
            SortDirection = QueryStringParser.RestoreSortDirection(returnParameters);
            SortField = QueryStringParser.RestoreSortField(returnParameters);
            RestoreCustomModelState(returnParameters);
        }

        protected virtual void RestoreDefaultModelState()
        {
        }

        public RouteValueDictionary ToRouteValueDictionary()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add(QueryStringParser.GridPageIndexParameterName, PageIndex);
            dict.Add(QueryStringParser.GridSortFieldParameterName, SortField);
            dict.Add(QueryStringParser.GridSortDirectionParameterName, SortDirection);

            AddCustomRouteValues(dict);

            return new RouteValueDictionary(dict);
        }

        public string DecodeModelParameters()
        {
            string coreParameters = DecodeModelParametersCore();
            return string.Format("{0}&{1}", QueryStringParser.DecodeGridParameters(PageIndex, SortField, SortDirection),
                coreParameters);
        }

        protected virtual void ParseCustomModelState(HttpRequestBase request)
        {
        }

        protected virtual void RestoreCustomModelState(string returnParameters)
        {
        }

        protected virtual string DecodeModelParametersCore()
        {
            return "";
        }

        protected virtual void AddCustomRouteValues(IDictionary<string, object> dictionary)
        {
        }
    }
}