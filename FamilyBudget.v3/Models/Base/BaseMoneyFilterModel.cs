using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FamilyBudget.v3.Models.Base
{
    public abstract class BaseMoneyFilterModel : BaseFilterModel
    {
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Счет")]
        public int AccountId { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}