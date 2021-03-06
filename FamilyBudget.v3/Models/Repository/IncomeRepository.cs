﻿using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Repository.Interfaces;

namespace FamilyBudget.v3.Models.Repository
{
    public class IncomeRepository : GenericRepository<FamilyBudgetEntities, Income>, IIncomeRepository
    {
    }
}