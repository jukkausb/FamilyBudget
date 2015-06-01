using System.Collections.Generic;
using FamilyBudget.Www.App_Helpers;

namespace FamilyBudget.Www.Models.Base
{
    public abstract class BaseListModel<T, TK> : BaseListAwareModel<T, TK> where TK : BaseFilterModel
    {
        protected BaseListModel()
        {
            PageSize = 20;
        }

        /// <summary>
        ///     Page size of the grid displaying list of entities
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Entity collection
        /// </summary>
        public List<T> Entities { get; set; }

        public string EncodeModelParameters()
        {
            string coreParameters = EncodeModelParametersCore();
            return string.Format("{0};{1}", QueryStringParser.EncodeGridParameters(PageIndex, SortField, SortDirection),
                coreParameters);
        }

        protected virtual string EncodeModelParametersCore()
        {
            return string.Empty;
        }
    }
}