using FamilyBudget.v3.Models.Base;

namespace FamilyBudget.v3.Models
{
    public class MoneyModel : BaseModel
    {
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string ValuePresentation { get; set; }
    }
}