//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FamilyBudget.Www.App_DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExpenditureCategory
    {
        public ExpenditureCategory()
        {
            this.Expenditure = new HashSet<Expenditure>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Expenditure> Expenditure { get; set; }
    }
}
