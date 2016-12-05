using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Next.Data
{
    public class Account
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CurrencyID { get; set; }
        public decimal Balance { get; set; }
        public bool IsMain { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ICollection<Expenditure> Expenditure { get; set; }
        public virtual ICollection<Income> Income { get; set; }
    }
}
