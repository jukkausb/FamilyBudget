using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models.Base;
using System;

namespace FamilyBudget.v3.Models
{
    public class MoneyModel : BaseModel
    {
        public double Value { get; set; }
        public string Currency { get; set; }
        public string ValuePresentation
        {
            get
            {
                return Math.Abs(Value).ToCurrencyDisplay(Currency, true);
            }
        }
    }
}