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
    
    public partial class Currency
    {
        public Currency()
        {
            this.Account = new HashSet<Account>();
        }
    
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Account> Account { get; set; }
    }
}