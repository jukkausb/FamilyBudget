using System.Web.Mvc;

namespace FamilyBudget.v3.App_CodeBase
{
    public class ExtendedSelectListItem : SelectListItem
    {
        public bool IsBold { get; set; }
        public object HtmlAttributes { get; set; }
    }
}