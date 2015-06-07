using System;
using System.Linq;
using FamilyBudget.Www.App_CodeBase;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Repository.Interfaces;
using System.Collections.Generic;
using System.Globalization;

namespace FamilyBudget.Www.Controllers
{
    public class MoneyControllerBase<T> : BaseController where T : class, IAccountableEntity
    {
        private IAccountRepository _acountRepository;

        public MoneyControllerBase(IAccountRepository acountRepository)
        {
            _acountRepository = acountRepository;
        }

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
            ChangeAccountBalance(AccountRepository.FindBy(a => a.ID == accountId).FirstOrDefault(), accountableEntity);
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
            RestoreAccountBalance(AccountRepository.FindBy(a => a.ID == accountId).FirstOrDefault(), accountableEntity);
        }

        protected T FindAndRestoreAccountBalance(T accountableEntity)
        {
            Account accountToRestoreBalance = AccountRepository.FindBy(a => a.ID == accountableEntity.AccountID).FirstOrDefault();
            if (accountToRestoreBalance == null)
            {
                throw new Exception("No account found to restore balance");
            }

            T entityToRestore = AccountRepository.Context.Set<T>().Find(accountableEntity.ID);
            if (entityToRestore == null)
            {
                throw new Exception("No entity found to restore balance");
            }

            RestoreAccountBalance(accountToRestoreBalance, entityToRestore);

            return entityToRestore;
        }

        protected List<ExtendedSelectListItem> GetAccountsForDropDownExtended()
        {
            List<ExtendedSelectListItem> accounts = _acountRepository.GetAll().Select(c => new ExtendedSelectListItem
            {
                IsBold = c.IsMain,
                Value = c.ID.ToString(CultureInfo.InvariantCulture),
                Text = c.DisplayName,
                Selected = c.IsMain,
                HtmlAttributes = new { data_currency = c.Currency.Code }
            }).ToList();

            accounts.Insert(0, new ExtendedSelectListItem { Text = " - Выберите счет - ", Value = "" });
            return accounts;
        }
    }
}