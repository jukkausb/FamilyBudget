using System;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;

namespace FamilyBudget.Www.Controllers
{
    public class MoneyControllerBase<T> : BaseController where T : class, IAccountableEntity
    {
        private static void CheckChangeBalanceRequest(Account account, IAccountableEntity accountableEntity)
        {
            if (accountableEntity == null)
            {
                throw new Exception("No entity found to change balance");
            }

            if (account == null)
            {
                throw new Exception("No account found to decrease balance");
            }
        }

        private static void IncreaseAccountBalance(Account account, IAccountableEntity accountableEntity)
        {
            account.Balance += accountableEntity.Summa;
        }

        private static void DecreaseAccountBalance(Account account, IAccountableEntity accountableEntity)
        {
            account.Balance -= accountableEntity.Summa;
        }

        protected void ChangeAccountBalance(Account account, IAccountableEntity accountableEntity)
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

        protected void ChangeAccountBalance(int accountId, IAccountableEntity accountableEntity)
        {
            ChangeAccountBalance(DbModelFamilyBudgetEntities.Account.Find(accountableEntity.AccountID),
                accountableEntity);
        }

        protected void RestoreAccountBalance(Account account, IAccountableEntity accountableEntity)
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

        protected void RestoreAccountBalance(int accountId, IAccountableEntity accountableEntity)
        {
            RestoreAccountBalance(DbModelFamilyBudgetEntities.Account.Find(accountableEntity.AccountID),
                accountableEntity);
        }

        protected T FindAndRestoreAccountBalance(T accountableEntity)
        {
            Account accountToRestoreBalance =
                DbModelFamilyBudgetEntities.Account.Find(accountableEntity.AccountID);
            if (accountToRestoreBalance == null)
            {
                throw new Exception("No account found to restore balance");
            }

            T expenditureToRestore =
                DbModelFamilyBudgetEntities.Set<T>().Find(accountableEntity.ID);
            if (expenditureToRestore == null)
            {
                throw new Exception("No expenditure found to restore balance");
            }

            RestoreAccountBalance(accountToRestoreBalance, expenditureToRestore);

            return expenditureToRestore;
        }
    }
}