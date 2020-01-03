using System.Collections.Generic;
using System.Web.Mvc;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Areas.Administration.Models
{
    public class AccountModel : BaseListAwareModel<Account>
    {
        public IEnumerable<SelectListItem> Currencies { get; set; }
    }
}