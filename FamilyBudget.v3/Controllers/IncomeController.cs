﻿using FamilyBudget.v3.App_CodeBase;
using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Controllers.Services;
using FamilyBudget.v3.Models;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web.Mvc;

namespace FamilyBudget.v3.Controllers
{
    public class IncomeController : MoneyControllerBase<Income>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IIncomeCategoryRepository _incomeCategoryRepository;
        private readonly IIncomeSuggestionService _incomeSuggestionService;
        

        public IncomeController(IAccountRepository acountRepository, IIncomeRepository incomeRepository, IIncomeCategoryRepository incomeCategoryRepository, IIncomeSuggestionService incomeSuggestionService)
        {
            _accountRepository = acountRepository;
            _incomeRepository = incomeRepository;
            _incomeCategoryRepository = incomeCategoryRepository;
            _incomeSuggestionService = incomeSuggestionService;
        }

        public ViewResult Index(int? page, IncomeListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.InitializeFilter(
                    GetAccountsForDropDownExtended(_accountRepository),
                    GetCategoriesForDropDown(_incomeCategoryRepository.GetAll().ToList().Cast<ICategoryInfo>().ToList())
                    );
                IQueryable<Income> query = _incomeRepository.Context.Income.AsQueryable();

                if (!string.IsNullOrEmpty(listModel.Filter.Description))
                {
                    query = query.Where(i => i.Description.Contains(listModel.Filter.Description));
                }

                if (listModel.Filter.CategoryId > 0)
                {
                    query = query.Where(i => i.CategoryID == listModel.Filter.CategoryId);
                }

                if (listModel.Filter.AccountId > 0)
                {
                    query = query.Where(i => i.AccountID == listModel.Filter.AccountId);
                }

                listModel.Entities = query.ToList();

                ModelState.Clear();

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
                Accounts = GetAccountsForDropDownExtended(_accountRepository),
                DescriptionSuggestions = _incomeSuggestionService.GetTopNSuggestions()
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
                using (var scope = new TransactionScope())
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

            income.OldAccountID = income.AccountID;

            var model = new IncomeModel
            {
                Categories = GetIncomeCategories(),
                Accounts = GetAccountsForDropDownExtended(_accountRepository),
                DescriptionSuggestions = _incomeSuggestionService.GetTopNSuggestions(),
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
                using (var scope = new TransactionScope())
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
            Income income = _incomeRepository.FindBy(i => i.ID == id.Value).FirstOrDefault();
            if (income == null)
            {
                return HttpNotFound();
            }

            income.OldAccountID = income.AccountID;

            var model = new IncomeModel { Categories = GetIncomeCategories(), Object = income };
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

            using (var scope = new TransactionScope())
            {
                try
                {
                    FindAndRestoreAccountBalance(_accountRepository, incomeModel.Object);
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

        public ActionResult Copy(int? id)
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

            Income copy = new Income
            {
                AccountID = income.AccountID,
                CategoryID = income.CategoryID,
                Date = DateTime.Now.Date,
                Description = income.Description,
                Summa = income.Summa
            };

            var model = new IncomeModel
            {
                Categories = GetIncomeCategories(),
                Accounts = GetAccountsForDropDownExtended(_accountRepository),
                DescriptionSuggestions = _incomeSuggestionService.GetTopNSuggestions(),
                Object = copy
            };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(IncomeModel incomeModel, int submit)
        {
            if (incomeModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
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