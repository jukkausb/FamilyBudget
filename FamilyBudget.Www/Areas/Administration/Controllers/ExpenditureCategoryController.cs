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
    public class ExpenditureCategoryController : BaseController
    {
        public ViewResult Index(int? page, ExpenditureCategoryListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = DbModelFamilyBudgetEntities.ExpenditureCategory.ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new ExpenditureCategoryModel();
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExpenditureCategoryModel ExpenditureCategoryModel)
        {
            if (
                DbModelFamilyBudgetEntities.ExpenditureCategory.Any(
                    c =>
                        c.Name.Equals(ExpenditureCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != ExpenditureCategoryModel.Object.ID))
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.ExpenditureCategory.Add(ExpenditureCategoryModel.Object);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    ExpenditureCategoryModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", ExpenditureCategoryModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(ExpenditureCategoryModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ExpenditureCategory ExpenditureCategory = DbModelFamilyBudgetEntities.ExpenditureCategory.Find(id);
            if (ExpenditureCategory == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureCategoryModel();
            model.Object = ExpenditureCategory;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExpenditureCategoryModel ExpenditureCategoryModel)
        {
            if (
                DbModelFamilyBudgetEntities.ExpenditureCategory.Any(
                    c =>
                        c.Name.Equals(ExpenditureCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != ExpenditureCategoryModel.Object.ID))
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.Entry(ExpenditureCategoryModel.Object).State = EntityState.Modified;
                    DbModelFamilyBudgetEntities.SaveChanges();

                    ExpenditureCategoryModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", ExpenditureCategoryModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(ExpenditureCategoryModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpenditureCategory ExpenditureCategory = DbModelFamilyBudgetEntities.ExpenditureCategory.Find(id);
            if (ExpenditureCategory == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureCategoryModel();
            model.Object = ExpenditureCategory;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, ExpenditureCategoryModel ExpenditureCategoryModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ExpenditureCategory ExpenditureCategory = DbModelFamilyBudgetEntities.ExpenditureCategory.Find(id);
            if (ExpenditureCategory == null)
            {
                return HttpNotFound();
            }

            if (DbModelFamilyBudgetEntities.Expenditure.Any(e => e.CategoryID == ExpenditureCategory.ID))
            {
                ModelState.AddModelError("",
                    "Эта категория имеет связанные с ней расходы. Сначала удалите связанные расходы");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.ExpenditureCategory.Remove(ExpenditureCategory);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    ExpenditureCategoryModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", ExpenditureCategoryModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(ExpenditureCategoryModel);
        }
    }
}