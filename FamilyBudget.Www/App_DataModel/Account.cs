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
    
    public partial class Account
    {
        public Account()
        {
            this.Expenditure = new HashSet<Expenditure>();
            this.Income = new HashSet<Income>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int CurrencyID { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceReal { get; set; }
        public bool IsMain { get; set; }
    
        public virtual Currency Currency { get; set; }
        public virtual ICollection<Expenditure> Expenditure { get; set; }
        public virtual ICollection<Income> Income { get; set; }
    }
}
