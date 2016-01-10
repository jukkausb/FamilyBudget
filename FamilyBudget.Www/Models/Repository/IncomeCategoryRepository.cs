using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Repository.Interfaces;

namespace FamilyBudget.Www.Models.Repository
{
    public class IncomeCategoryRepository : GenericRepository<FamilyBudgetEntities, IncomeCategory>, IIncomeCategoryRepository
    {
    }
}