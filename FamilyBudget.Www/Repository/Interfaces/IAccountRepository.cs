using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FamilyBudget.Www.Repository.Interfaces
{
    public interface IAccountRepository : IGenericRepository<FamilyBudgetEntities, Account>
    {
        void ChangeAccountBalance(Account account, IAccountableEntity accountableEntity);
        void ChangeAccountBalance(int accountId, IAccountableEntity accountableEntity);
        void RestoreAccountBalance(Account account, IAccountableEntity accountableEntity);
        void RestoreAccountBalance(int accountId, IAccountableEntity accountableEntity);
    }
}
