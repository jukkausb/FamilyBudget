using FamilyBudget.Www.App_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.Www.Repository.Interfaces
{
    public interface IIncomeCategoryRepository : IGenericRepository<FamilyBudgetEntities, IncomeCategory>
    {
    }
}