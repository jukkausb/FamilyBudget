using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Repository.Interfaces;

namespace FamilyBudget.Www.Models.Repository
{
    public class IncomeRepository : GenericRepository<FamilyBudgetEntities, Income>, IIncomeRepository
    {
    }
}