using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace FamilyBudget.v3.App_Helpers
{
    /// <summary>
    ///     Represents web grid helper class
    /// </summary>
    public static class WebGridHelpers
    {
        /// <summary>
        ///     Constructs web grid
        /// </summary>
        /// <returns>Web grid instance</returns>
        public static WebGrid CreateWebGridWithJqueryUiStyle(IEnumerable<dynamic> source = null,
            IEnumerable<string> columnNames = null, string defaultSort = null, int rowsPerPage = 100,
            bool canPage = false, bool canSort = false, string ajaxUpdateContainerId = null,
            string ajaxUpdateCallback = null, string fieldNamePrefix = null, string pageFieldName = null,
            string selectionFieldName = null, string sortFieldName = null, string sortDirectionFieldName = null)
        {
            return new WebGrid(source, columnNames, defaultSort, rowsPerPage, canPage, canSort, ajaxUpdateContainerId,
                ajaxUpdateCallback, fieldNamePrefix, pageFieldName, selectionFieldName, sortFieldName,
                sortDirectionFieldName);
        }

        /// <summary>
        ///     Builds generic web grid
        /// </summary>
        /// <returns>Web grid markup</returns>
        public static IHtmlString GetHtmlJqueryUiStyle(this WebGrid webGrid,
            string tableStyle = "table table-hover table-condensed table-striped table-bordered",
            string selectedRowStyle = null,
            string caption = null,
            bool displayHeader = true,
            bool fillEmptyRows = false,
            string emptyRowCellValue = null,
            IEnumerable<WebGridColumn> columns = null,
            IEnumerable<string> exclusions = null,
            WebGridPagerModes mode = WebGridPagerModes.Numeric | WebGridPagerModes.NextPrevious,
            int numericLinksCount = 5,
            object htmlAttributes = null,
            bool showNoDataFound = false)
        {
            const string pagerPreviousText = "Пред.";
            const string pagerNextText = "След.";
            const string pagerFirstText = "Первая";
            const string pagerLastText = "Последняя";

            IHtmlString html = webGrid.GetHtml(tableStyle, "", "", "", "", selectedRowStyle, caption, displayHeader,
                fillEmptyRows, emptyRowCellValue, columns.Where(c => c != null), exclusions, mode, pagerFirstText,
                pagerPreviousText, pagerNextText, pagerLastText, numericLinksCount, htmlAttributes);
            if (showNoDataFound && webGrid.Rows.Count == 0)
            {
                string noDataFound = string.Format("<div class=\"nodatafound\">{0}</div>", "No data found");
                html = new HtmlString(html + noDataFound);
            }

            return html;
        }
    }
}