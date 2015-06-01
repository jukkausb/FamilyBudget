using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Areas.Administration.Models;
using FamilyBudget.Www.Controllers;

namespace FamilyBudget.Www.Areas.Administration.Controllers
{
    public class AccountController : BaseController
    {
        private void MarkAccountAsMain(Account account)
        {
            if (account == null)
            {
                return;
            }

            account.IsMain = true;
            DbModelFamilyBudgetEntities.Account.Where(a => a.ID != account.ID)
                .ToList()
                .ForEach(acc => acc.IsMain = false);
        }

        public ViewResult Index(int? page, AccountListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = DbModelFamilyBudgetEntities.Account.ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new AccountModel {Currencies = GetCurrencies()};
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountModel accountModel)
        {
            if (
                DbModelFamilyBudgetEntities.Account.Any(
                    c => c.CurrencyID == accountModel.Object.CurrencyID && c.ID != accountModel.Object.ID))
            {
                ModelState.AddModelError("", "Счет с таким кодом валюты уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (accountModel.Object.IsMain)
                    {
                        MarkAccountAsMain(accountModel.Object);
                    }

                    DbModelFamilyBudgetEntities.Account.Add(accountModel.Object);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    accountModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", accountModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            else
            {
                accountModel.Currencies = GetCurrencies();
            }

            return View(accountModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Account account = DbModelFamilyBudgetEntities.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            var model = new AccountModel {Object = account, Currencies = GetCurrencies()};
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountModel accountModel)
        {
            if (
                DbModelFamilyBudgetEntities.Account.Any(
                    c => c.CurrencyID == accountModel.Object.CurrencyID && c.ID != accountModel.Object.ID))
            {
                ModelState.AddModelError("", "Счет с таким кодом валюты уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (accountModel.Object.IsMain)
                    {
                        MarkAccountAsMain(accountModel.Object);
                    }

                    DbModelFamilyBudgetEntities.Entry(accountModel.Object).State = EntityState.Modified;
                    DbModelFamilyBudgetEntities.SaveChanges();
                    accountModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", accountModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            else
            {
                accountModel.Currencies = GetCurrencies();
            }

            return View(accountModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Account account = DbModelFamilyBudgetEntities.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            var model = new AccountModel {Object = account};
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, AccountModel accountModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Account account = DbModelFamilyBudgetEntities.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            if (account.Balance != 0)
            {
                ModelState.AddModelError("", "Этот счет имеет ненулевой остаток. Сначала обнулите остаток");
            }

            if (account.Expenditure.Any() || account.Income.Any())
            {
                ModelState.AddModelError("", "Этот счет имеет связанные расходы или доходы. Сначала удалите их");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (accountModel.Object.IsMain)
                    {
                        MarkAccountAsMain(DbModelFamilyBudgetEntities.Account.FirstOrDefault());
                    }

                    DbModelFamilyBudgetEntities.Account.Remove(account);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    accountModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", accountModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            else
            {
                accountModel.Currencies = GetCurrencies();
            }

            return View(accountModel);
        }
    }
}