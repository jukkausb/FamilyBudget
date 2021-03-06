﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FamilyBudget.v3.App_DataModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FamilyBudgetEntities : DbContext
    {
        public FamilyBudgetEntities()
            : base("name=FamilyBudgetEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Expenditure> Expenditure { get; set; }
        public virtual DbSet<ExpenditureCategory> ExpenditureCategory { get; set; }
        public virtual DbSet<Income> Income { get; set; }
        public virtual DbSet<IncomeCategory> IncomeCategory { get; set; }
        public virtual DbSet<InvestmentInstrument> InvestmentInstrument { get; set; }
        public virtual DbSet<InvestmentInstrumentMarket> InvestmentInstrumentMarket { get; set; }
        public virtual DbSet<InvestmentInstrumentType> InvestmentInstrumentType { get; set; }
    }
}
