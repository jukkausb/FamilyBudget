using System;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Models;

namespace FamilyBudget.Www.Controllers
{
    public class ExpenditureController : MoneyControllerBase<Expenditure>
    {
        public ViewResult Index(int? page, ExpenditureListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.InitializeFilter(GetAccountsForDropDownExtended());
                IQueryable<Expenditure> query = DbModelFamilyBudgetEntities.Expenditure.AsQueryable();

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
                using (var context = new FamilyBudgetEntities())
                {
                    context.Database.Connection.Open();
                    using (DbTransaction dbContextTransaction = context.Database.Connection.BeginTransaction())
                    {
                        try
                        {
                            DbModelFamilyBudgetEntities.Expenditure.Add(expenditureModel.Object);
                            ChangeAccountBalance(expenditureModel.Object.AccountID, expenditureModel.Object);
                            DbModelFamilyBudgetEntities.SaveChanges();
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

            Expenditure expenditure = DbModelFamilyBudgetEntities.Expenditure.Find(id);
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
                    using (var context = new FamilyBudgetEntities())
                    {
                        context.Database.Connection.Open();
                        using (dbContextTransaction = context.Database.Connection.BeginTransaction())
                        {
                            Expenditure expenditureToRestore = FindAndRestoreAccountBalance(expenditureModel.Object);
                            Expenditure.Copy(expenditureToRestore, expenditureModel.Object);
                            ChangeAccountBalance(expenditureToRestore.Account.ID, expenditureModel.Object);
                            DbModelFamilyBudgetEntities.SaveChanges();

                            dbContextTransaction.Commit();
                            expenditureModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                            return RedirectToAction("Index", expenditureModel.ToRouteValueDictionary());
                        }
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
            Expenditure expenditure = DbModelFamilyBudgetEntities.Expenditure.Find(id);
            if (expenditure == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureModel {Categories = GetExpenditureCategories(), Object = expenditure};
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, ExpenditureModel expenditureModel)
        {
            Expenditure expenditure = DbModelFamilyBudgetEntities.Expenditure.Find(id);
            if (expenditure == null)
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
                        FindAndRestoreAccountBalance(expenditure);
                        DbModelFamilyBudgetEntities.Expenditure.Remove(expenditure);
                        DbModelFamilyBudgetEntities.SaveChanges();
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
            }

            return View(expenditureModel);
        }
    }
}