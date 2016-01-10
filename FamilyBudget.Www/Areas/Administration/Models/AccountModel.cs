using System.Collections.Generic;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.Areas.Administration.Models
{
    public class AccountModel : BaseListAwareModel<Account>
    {
        public IEnumerable<SelectListItem> Currencies { get; set; }
    }
}