using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_DataModel;

namespace FamilyBudget.v3.Models.Repository.Interfaces
{
    public interface IAccountRepository : IGenericRepository<FamilyBudgetEntities, Account>
    {
        void ChangeAccountBalance(Account account, IAccountableEntity accountableEntity);
        void ChangeAccountBalance(int accountId, IAccountableEntity accountableEntity);
        void RestoreAccountBalance(Account account, IAccountableEntity accountableEntity);
        void RestoreAccountBalance(int accountId, IAccountableEntity accountableEntity);
    }
}
