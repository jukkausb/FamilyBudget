using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.Models.Filter
{
    public class ExpenditureFilterModel : BaseFilterModel
    {
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Счет")]
        public int AccountId { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}