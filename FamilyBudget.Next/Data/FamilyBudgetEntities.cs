using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Next.Data
{
    public class FamilyBudgetEntities : DbContext
    {
        public FamilyBudgetEntities(DbContextOptions<FamilyBudgetEntities> options) : base(options)
        {
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Expenditure> Expenditure { get; set; }
        public DbSet<ExpenditureCategory> ExpenditureCategory { get; set; }
        public DbSet<Income> Income { get; set; }
        public DbSet<IncomeCategory> IncomeCategory { get; set; }
    }
}
