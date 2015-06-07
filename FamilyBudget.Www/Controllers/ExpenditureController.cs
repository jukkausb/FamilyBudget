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
    public class ExpenditureController : MoneyControllerBase<Expenditure>
    {
        private IAccountRepository _acountRepository;
        private IExpenditureRepository _expenditureRepository;
        private IExpenditureCategoryRepository _expenditureCategoryRepository;

        public ExpenditureController(IAccountRepository acountRepository, IExpenditureRepository expenditureRepository, IExpenditureCategoryRepository expenditureCategoryRepository)
            : base(acountRepository)
        {
            _acountRepository = acountRepository;
            _expenditureRepository = expenditureRepository;
            _expenditureCategoryRepository = expenditureCategoryRepository;
        }

        public ViewResult Index(int? page, ExpenditureListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.InitializeFilter(GetAccountsForDropDownExtended());
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
                Accounts = GetAccountsForDropDownExtended()
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
                _expenditureRepository.Context.Database.Connection.Open();
                using (DbTransaction dbContextTransaction = _expenditureRepository.Context.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        _expenditureRepository.Add(expenditureModel.Object);
                        ChangeAccountBalance(expenditureModel.Object.AccountID, expenditureModel.Object);
                        _expenditureRepository.SaveChanges();
                        dbContextTransaction.Commit();
                        expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        expenditureModel.Categories = GetExpenditureCategories();
                        expenditureModel.Accounts = GetAccountsForDropDownExtended();
                        HandleException(ex);
                    }
                }
            }
            else
            {
                expenditureModel.Categories = GetExpenditureCategories();
                expenditureModel.Accounts = GetAccountsForDropDownExtended();
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
                Accounts = GetAccountsForDropDownExtended(),
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
                DbTransaction dbContextTransaction = null;
                try
                {
                    _expenditureRepository.Context.Database.Connection.Open();
                    using (dbContextTransaction = _expenditureRepository.Context.Database.Connection.BeginTransaction())
                    {
                        Expenditure expenditureToRestore = FindAndRestoreAccountBalance(expenditureModel.Object);
                        Expenditure.Copy(expenditureToRestore, expenditureModel.Object);
                        ChangeAccountBalance(expenditureToRestore.Account.ID, expenditureModel.Object);
                        _expenditureRepository.SaveChanges();

                        dbContextTransaction.Commit();
                        expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                        return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                    }
                }
                catch (Exception ex)
                {
                    if (dbContextTransaction != null)
                    {
                        dbContextTransaction.Rollback();
                    }
                    expenditureModel.Categories = GetExpenditureCategories();
                    expenditureModel.Accounts = GetAccountsForDropDownExtended();
                    HandleException(ex);
                }
            }
            else
            {
                expenditureModel.Categories = GetExpenditureCategories();
                expenditureModel.Accounts = GetAccountsForDropDownExtended();
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

            _expenditureRepository.Context.Database.Connection.Open();
            using (DbTransaction dbContextTransaction = _expenditureRepository.Context.Database.Connection.BeginTransaction())
            {
                try
                {
                    FindAndRestoreAccountBalance(expenditure);
                    _expenditureRepository.Delete(expenditure);
                    _expenditureRepository.SaveChanges();
                    expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
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