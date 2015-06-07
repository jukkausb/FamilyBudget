using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyBudget.Www.Repository
{
    public class AccountRepository : GenericRepository<FamilyBudgetEntities, Account>, IAccountRepository
    {
        public void RestoreAccountBalance(int accountId, IAccountableEntity accountableEntity)
        {
            RestoreAccountBalance(FindBy(a => a.ID == accountId).FirstOrDefault(), accountableEntity);
        }

        public void RestoreAccountBalance(Account account, IAccountableEntity accountableEntity)
        {
            if (account == null || accountableEntity == null)
                return;

            switch (accountableEntity.AccountRestoreOperationType)
            {
                case AccountRestoreOperation.Increase:
                    IncreaseAccountBalance(account, accountableEntity);
                    break;
                case AccountRestoreOperation.Decrease:
                    DecreaseAccountBalance(account, accountableEntity);
                    break;
            }
        }

        public void ChangeAccountBalance(int accountId, IAccountableEntity accountableEntity)
        {
            ChangeAccountBalance(FindBy(a => a.ID == accountId).FirstOrDefault(), accountableEntity);
        }

        public void ChangeAccountBalance(Account account, IAccountableEntity accountableEntity)
        {
            CheckChangeBalanceRequest(account, accountableEntity);
            switch (accountableEntity.AccountTransactionOperationType)
            {
                case AccountTransactionOperation.Increase:
                    IncreaseAccountBalance(account, accountableEntity);
                    break;
                case AccountTransactionOperation.Decrease:
                    DecreaseAccountBalance(account, accountableEntity);
                    break;
            }
        }

        private void CheckChangeBalanceRequest(Account account, IAccountableEntity accountableEntity)
        {
            if (accountableEntity == null)
            {
                throw new Exception("No entity found to change balance");
            }

            if (account == null)
            {
                throw new Exception("No account found to change balance");
            }
        }

        private void IncreaseAccountBalance(Account account, IAccountableEntity accountableEntity)
        {
            account.Balance += accountableEntity.Summa;
        }

        private void DecreaseAccountBalance(Account account, IAccountableEntity accountableEntity)
        {
            account.Balance -= accountableEntity.Summa;
        }

    }
}