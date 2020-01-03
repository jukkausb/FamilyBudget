using System;
using System.Linq;
using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System.Collections.Generic;
using System.Globalization;

namespace FamilyBudget.v3.Controllers
{
    public class MoneyControllerBase<T> : BaseController where T : class, IAccountableEntity
    {
        protected T FindAndRestoreAccountBalance(IAccountRepository accountRepository, T accountableEntity)
        {
            Account accountToRestoreBalance = accountRepository.FindBy(a => a.ID == accountableEntity.OldAccountID).FirstOrDefault();
            if (accountToRestoreBalance == null)
            {
                throw new Exception("No account found to restore balance");
            }

            T entityToRestore = accountRepository.Context.Set<T>().Find(accountableEntity.ID);
            if (entityToRestore == null)
            {
                throw new Exception("No entity found to restore balance");
            }

            accountRepository.RestoreAccountBalance(accountToRestoreBalance, entityToRestore);

            return entityToRestore;
        }

        protected List<ExtendedSelectListItem> GetAccountsForDropDownExtended(IAccountRepository accountRepository)
        {
            List<ExtendedSelectListItem> accounts = accountRepository.GetAll().ToList().Select(c => new ExtendedSelectListItem
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

        protected List<ExtendedSelectListItem> GetCategoriesForDropDown(List<ICategoryInfo> categories)
        {
            List<ExtendedSelectListItem> accounts = categories.Select(c => new ExtendedSelectListItem
            {
                Value = c.ID.ToString(CultureInfo.InvariantCulture),
                Text = c.Name
            }).ToList();

            accounts.Insert(0, new ExtendedSelectListItem { Text = " - Выберите категорию - ", Value = "" });
            return accounts;
        }
    }
}