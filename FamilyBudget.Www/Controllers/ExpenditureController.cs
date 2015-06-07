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
using System.Transactions;

namespace FamilyBudget.Www.Controllers
{
    public class ExpenditureController : MoneyControllerBase<Expenditure>
    {
        private IAccountRepository _accountRepository;
        private IExpenditureRepository _expenditureRepository;
        private IExpenditureCategoryRepository _expenditureCategoryRepository;

        public ExpenditureController(IAccountRepository acountRepository, IExpenditureRepository expenditureRepository, IExpenditureCategoryRepository expenditureCategoryRepository)
        {
            _accountRepository = acountRepository;
            _expenditureRepository = expenditureRepository;
            _expenditureCategoryRepository = expenditureCategoryRepository;
        }

        public ViewResult Index(int? page, ExpenditureListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.InitializeFilter(GetAccountsForDropDownExtended(_accountRepository));
                IQueryable<Expenditure> query = _expenditureRepository.Context.Expenditure.AsQueryable();

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
            var model = new ExpenditureModel
            {
                Categories = GetExpenditureCategories(),
                Accounts = GetAccountsForDropDownExtended(_accountRepository)
            };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExpenditureModel expenditureModel)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        _expenditureRepository.Add(expenditureModel.Object);
                        _accountRepository.ChangeAccountBalance(expenditureModel.Object.AccountID, expenditureModel.Object);
                        _expenditureRepository.SaveChanges();
                        _accountRepository.SaveChanges();

                        scope.Complete();

                        expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                    }
                    catch (Exception ex)
                    {
                        expenditureModel.Categories = GetExpenditureCategories();
                        expenditureModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
                        HandleException(ex);
                    }
                }
            }
            else
            {
                expenditureModel.Categories = GetExpenditureCategories();
                expenditureModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
            }

            return View(expenditureModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Expenditure expenditure = _expenditureRepository.FindBy(e => e.ID == id).FirstOrDefault();
            if (expenditure == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureModel
            {
                Categories = GetExpenditureCategories(),
                Accounts = GetAccountsForDropDownExtended(_accountRepository),
                Object = expenditure
            };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExpenditureModel expenditureModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Expenditure expenditureToRestore = FindAndRestoreAccountBalance(_accountRepository, expenditureModel.Object);
                        Expenditure.Copy(expenditureToRestore, expenditureModel.Object);
                        _accountRepository.ChangeAccountBalance(expenditureToRestore.AccountID, expenditureModel.Object);
                        _expenditureRepository.SaveChanges();
                        _accountRepository.SaveChanges();

                        scope.Complete();

                        expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                    }
                }
                catch (Exception ex)
                {
                    expenditureModel.Categories = GetExpenditureCategories();
                    expenditureModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
                    HandleException(ex);
                }
            }
            else
            {
                expenditureModel.Categories = GetExpenditureCategories();
                expenditureModel.Accounts = GetAccountsForDropDownExtended(_accountRepository);
            }

            return View(expenditureModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expenditure expenditure = _expenditureRepository.FindBy(e => e.ID == id).FirstOrDefault();
            if (expenditure == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureModel { Categories = GetExpenditureCategories(), Object = expenditure };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, ExpenditureModel expenditureModel)
        {
            Expenditure expenditure = _expenditureRepository.FindBy(e => e.ID == id).FirstOrDefault();
            if (expenditure == null)
            {
                return HttpNotFound();
            }

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    FindAndRestoreAccountBalance(_accountRepository, expenditure);
                    _expenditureRepository.Delete(expenditure);
                    _expenditureRepository.SaveChanges();
                    _accountRepository.SaveChanges();

                    scope.Complete();

                    expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    expenditureModel.Categories = GetExpenditureCategories();
                    HandleException(ex);
                }
            }

            return View(expenditureModel);
        }

        protected List<SelectListItem> GetExpenditureCategories()
        {
            List<SelectListItem> categories =
                _expenditureCategoryRepository.GetAll().ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0}", c.Name)
                }).ToList();

            categories.Insert(0, new SelectListItem { Text = " - Выберите категорию - ", Value = "" });
            return categories;
        }
    }
}