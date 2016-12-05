using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Next.Data
{
    public class Currency
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
