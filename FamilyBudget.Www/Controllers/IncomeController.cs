﻿using System;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Models;
using System.Collections.Generic;
using FamilyBudget.Www.Repository.Interfaces;
using System.Globalization;
using System.Transactions;

namespace FamilyBudget.Www.Controllers
{
    public class IncomeController : MoneyControllerBase<Income>
    {
        private IAccountRepository _accountRepository;
        private IIncomeRepository _incomeRepository;
        private IIncomeCategoryRepository _incomeCategoryRepository;

        public IncomeController(IAccountRepository acountRepository, IIncomeRepository incomeRepository, IIncomeCategoryRepository incomeCategoryRepository)
        {
            _accountRepository = acountRepository;
            _incomeRepository = incomeRepository;
            _incomeCategoryRepository = incomeCategoryRepository;
        }

        public ViewResult Index(int? page, IncomeListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.InitializeFilter(GetAccountsForDropDownExtended(_accountRepository));
                IQueryable<Income> query = _incomeRepository.Context.Income.AsQueryable();

                if (!string.IsNullOrEmpty(listModel.Filter.Description))
                {
                    query = query.Where(i => i.Description.Contains(listModel.Filter.Description));
                }

                if (listModel.Filter.AccountId > 0)
                {
                    query = query.Where(i => i.AccountID == listModel.Filter.AccountId);
                }

                listModel.Entities = query.ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new IncomeModel
            {
                Categories = GetIncomeCategories(),
                Accounts = GetAccountsForDropDownExtended(_accountRepository)
            };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IncomeModel incomeModel, int submit)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        _incomeRepository.Add(incomeModel.Object);
                        _accountRepository.ChangeAccountBalance(incomeModel.Object.AccountID, incomeModel.Object);
                        _incomeRepository.SaveChanges();
                        _accountRepository.SaveChanges();

                        scope.Complete();

                        incomeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        switch (submit)
                        {
                            case 1:
                                return RedirectToAction("Index", incomeModel.ToRouteValueDictionary());
                            case 2:
                                return RedirectToAction("Create",
                                    new { returnParams = Request[QueryStringParser.GridReturnParameters] });
                        }
                    }
                    catch (Exception ex)
                    {
                        incomeModel.Categories = GetIncomeCategories();
                        incomeModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
                        HandleException(ex);
                    }
                }
            }
            else
            {
                incomeModel.Categories = GetIncomeCategories();
                incomeModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
            }

            return View(incomeModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Income income = _incomeRepository.FindBy(i => i.ID == id.Value).FirstOrDefault();
            if (income == null)
            {
                return HttpNotFound();
            }

            var model = new IncomeModel
            {
                Categories = GetIncomeCategories(),
                Accounts = GetAccountsForDropDownExtended(_accountRepository),
                Object = income
            };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IncomeModel incomeModel)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        Income incomeToRestore = FindAndRestoreAccountBalance(_accountRepository, incomeModel.Object);
                        Income.Copy(incomeToRestore, incomeModel.Object);
                        _accountRepository.ChangeAccountBalance(incomeToRestore.AccountID, incomeModel.Object);
                        _incomeRepository.SaveChanges();
                        _accountRepository.SaveChanges();

                        scope.Complete();

                        incomeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", incomeModel.ToRouteValueDictionary());
                    }
                    catch (Exception ex)
                    {
                        incomeModel.Categories = GetIncomeCategories();
                        incomeModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
                        HandleException(ex);
                    }
                }
            }
            else
            {
                incomeModel.Categories = GetIncomeCategories();
                incomeModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
            }

            return View(incomeModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Income Income = _incomeRepository.FindBy(i => i.ID == id.Value).FirstOrDefault();
            if (Income == null)
            {
                return HttpNotFound();
            }

            var model = new IncomeModel { Categories = GetIncomeCategories(), Object = Income };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, IncomeModel incomeModel)
        {
            Income income = _incomeRepository.FindBy(i => i.ID == id.Value).FirstOrDefault();
            if (income == null)
            {
                return HttpNotFound();
            }

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    FindAndRestoreAccountBalance(_accountRepository, income);
                    _incomeRepository.Delete(income);
                    _incomeRepository.SaveChanges();
                    _accountRepository.SaveChanges();

                    scope.Complete();

                    incomeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", incomeModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    incomeModel.Categories = GetIncomeCategories();
                    HandleException(ex);
                }
            }


            return View(incomeModel);
        }

        protected List<SelectListItem> GetIncomeCategories()
        {
            List<SelectListItem> categories =
                _incomeCategoryRepository.GetAll().ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0}", c.Name)
                }).ToList();

            categories.Insert(0, new SelectListItem { Text = " - Выберите категорию - ", Value = "" });
            return categories;
        }
    }
}