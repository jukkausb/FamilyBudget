using FamilyBudget.Www.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FamilyBudget.Www.Models.Filter
{
    public class ExpenditureFilterModel : BaseMoneyFilterModel
    {
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}