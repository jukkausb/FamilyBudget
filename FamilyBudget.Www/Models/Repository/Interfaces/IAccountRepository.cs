using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;

namespace FamilyBudget.Www.Models.Repository.Interfaces
{
    public interface IAccountRepository : IGenericRepository<FamilyBudgetEntities, Account>
    {
        void ChangeAccountBalance(Account account, IAccountableEntity accountableEntity);
        void ChangeAccountBalance(int accountId, IAccountableEntity accountableEntity);
        void RestoreAccountBalance(Account account, IAccountableEntity accountableEntity);
        void RestoreAccountBalance(int accountId, IAccountableEntity accountableEntity);
    }
}
