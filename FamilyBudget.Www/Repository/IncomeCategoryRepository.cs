using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.Www.Repository
{
    public class IncomeCategoryRepository : GenericRepository<FamilyBudgetEntities, IncomeCategory>, IIncomeCategoryRepository
    {
    }
}