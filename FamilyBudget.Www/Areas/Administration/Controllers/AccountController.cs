using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Areas.Administration.Models;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FamilyBudget.Www.Areas.Administration.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public AccountController(IAccountRepository accountRepository, ICurrencyRepository currencyRepository)
        {
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
        }

        private void MarkAccountAsMain(Account account)
        {
            if (account == null)
            {
                return;
            }

            account.IsMain = true;
            _accountRepository.FindBy(a => a.ID != account.ID)
                .ToList()
                .ForEach(acc => acc.IsMain = false);
        }

        public ViewResult Index(int? page, AccountListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = _accountRepository.GetAll().ToList();
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
                _accountRepository.FindBy(
                    c => c.CurrencyID == accountModel.Object.CurrencyID && c.ID != accountModel.Object.ID).Any())
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

                    _accountRepository.Add(accountModel.Object);
                    _accountRepository.SaveChanges();
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

            Account account = _accountRepository.FindBy(a => a.ID == id.Value).FirstOrDefault();
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
                _accountRepository.FindBy(
                    c => c.CurrencyID == accountModel.Object.CurrencyID && c.ID != accountModel.Object.ID).Any())
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

                    _accountRepository.Edit(accountModel.Object);
                    _accountRepository.SaveChanges();
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

            Account account = _accountRepository.FindBy(a => a.ID == id.Value).FirstOrDefault();
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

            Account account = _accountRepository.FindBy(a => a.ID == id.Value).FirstOrDefault();
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
                    _accountRepository.Delete(account);
                    _accountRepository.SaveChanges();

                    if (accountModel.Object.IsMain)
                    {
                        MarkAccountAsMain(_accountRepository.GetAll().FirstOrDefault());
                    }

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

        protected List<SelectListItem> GetCurrencies()
        {
            List<SelectListItem> currencies =
                _currencyRepository.GetAll().ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0} ({1})", c.Name, c.Code)
                }).ToList();

            currencies.Insert(0, new SelectListItem { Text = " - Выберите валюту - ", Value = "" });
            return currencies;
        }
    }
}