﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Next.Data
{
    public class Income
    {
        public int ID { get; set; }
        public decimal Summa { get; set; }
        public string Description { get; set; }

        public int CategoryID { get; set; }
        public System.DateTime Date { get; set; }
        public int AccountID { get; set; }

        public virtual Account Account { get; set; }
        public virtual IncomeCategory Category { get; set; }
    }
}