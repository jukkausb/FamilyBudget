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
        protected T FindAndRestoreAccountBalance(IAccountRepository _accountRepository, T accountableEntity)
        {
            Account accountToRestoreBalance = _accountRepository.FindBy(a => a.ID == accountableEntity.AccountID).FirstOrDefault();
            if (accountToRestoreBalance == null)
            {
                throw new Exception("No account found to restore balance");
            }

            T entityToRestore = _accountRepository.Context.Set<T>().Find(accountableEntity.ID);
            if (entityToRestore == null)
            {
                throw new Exception("No entity found to restore balance");
            }

            _accountRepository.RestoreAccountBalance(accountToRestoreBalance, entityToRestore);

            return entityToRestore;
        }

        protected List<ExtendedSelectListItem> GetAccountsForDropDownExtended(IAccountRepository _accountRepository)
        {
            List<ExtendedSelectListItem> accounts = _accountRepository.GetAll().ToList().Select(c => new ExtendedSelectListItem
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