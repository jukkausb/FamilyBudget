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
    
    public partial class Income
    {
        public int ID { get; set; }
        public decimal Summa { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public System.DateTime Date { get; set; }
        public int AccountID { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual IncomeCategory IncomeCategory { get; set; }
    }
}
