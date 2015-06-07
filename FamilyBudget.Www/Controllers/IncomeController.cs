using System;
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

namespace FamilyBudget.Www.Controllers
{
    public class IncomeController : MoneyControllerBase<Income>
    {
        private IAccountRepository _acountRepository;
        private IIncomeRepository _incomeRepository;
        private IIncomeCategoryRepository _incomeCategoryRepository;

        public IncomeController(IAccountRepository acountRepository, IIncomeRepository incomeRepository, IIncomeCategoryRepository incomeCategoryRepository)
            : base(acountRepository)
        {
            _acountRepository = acountRepository;
            _incomeRepository = incomeRepository;
            _incomeCategoryRepository = incomeCategoryRepository;
        }

        public ViewResult Index(int? page, IncomeListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.InitializeFilter(GetAccountsForDropDownExtended());
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
                Accounts = GetAccountsForDropDownExtended()
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
                _incomeRepository.Context.Database.Connection.Open();
                using (DbTransaction dbContextTransaction = _incomeRepository.Context.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        _incomeRepository.Add(incomeModel.Object);
                        ChangeAccountBalance(incomeModel.Object.AccountID, incomeModel.Object);
                        _incomeRepository.SaveChanges();
                        dbContextTransaction.Commit();
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
                        dbContextTransaction.Rollback();
                        incomeModel.Categories = GetIncomeCategories();
                        incomeModel.Accounts = GetAccountsForDropDownExtended();
                        HandleException(ex);
                    }
                }
            }
            else
            {
                incomeModel.Categories = GetIncomeCategories();
                incomeModel.Accounts = GetAccountsForDropDownExtended();
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
                Accounts = GetAccountsForDropDownExtended(),
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
                DbTransaction dbContextTransaction = null;
                try
                {
                    _incomeRepository.Context.Database.Connection.Open();
                    using (dbContextTransaction = _incomeRepository.Context.Database.Connection.BeginTransaction())
                    {
                        Income incomeToRestore = FindAndRestoreAccountBalance(incomeModel.Object);
                        Income.Copy(incomeToRestore, incomeModel.Object);
                        ChangeAccountBalance(incomeToRestore.Account.ID, incomeModel.Object);
                        _incomeRepository.SaveChanges();

                        dbContextTransaction.Commit();
                        incomeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", incomeModel.ToRouteValueDictionary());
                    }
                }
                catch (Exception ex)
                {
                    if (dbContextTransaction != null)
                    {
                        dbContextTransaction.Rollback();
                    }
                    incomeModel.Categories = GetIncomeCategories();
                    incomeModel.Accounts = GetAccountsForDropDownExtended();
                    HandleException(ex);
                }
            }
            else
            {
                incomeModel.Categories = GetIncomeCategories();
                incomeModel.Accounts = GetAccountsForDropDownExtended();
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
        public ActionResult Delete(int? id, IncomeModel IncomeModel)
        {
            Income Income = _incomeRepository.FindBy(i => i.ID == id.Value).FirstOrDefault();
            if (Income == null)
            {
                return HttpNotFound();
            }

            using (var context = new FamilyBudgetEntities())
            {
                context.Database.Connection.Open();
                using (DbTransaction dbContextTransaction = context.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        FindAndRestoreAccountBalance(Income);
                        _incomeRepository.Delete(Income);
                        _incomeRepository.SaveChanges();
                        IncomeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", IncomeModel.ToRouteValueDictionary());
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        IncomeModel.Categories = GetIncomeCategories();
                        HandleException(ex);
                    }
                }
            }


            return View(IncomeModel);
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