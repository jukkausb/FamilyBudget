using System.Web.Mvc;

namespace FamilyBudget.Www.App_CodeBase
{
    public class ExtendedSelectListItem : SelectListItem
    {
        public bool IsBold { get; set; }
        public object HtmlAttributes { get; set; }
    }
}