using System.Collections.Generic;

namespace FamilyBudget.v3.Models.Base
{
    public class BaseListModelSimple<T> : BaseModel
    {
        public List<T> Entities { get; set; }
    }
}