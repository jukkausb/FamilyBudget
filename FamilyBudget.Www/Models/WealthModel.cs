using FamilyBudget.Www.Models.Base;

namespace FamilyBudget.Www.Models
{
    public class WealthModel : BaseModel
    {
        public decimal Value { get; set; }
        public string Currency { get; set; }
    }
}