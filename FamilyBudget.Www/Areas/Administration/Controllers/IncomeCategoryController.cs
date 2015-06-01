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
    public class IncomeCategoryController : BaseController
    {
        public ViewResult Index(int? page, IncomeCategoryListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = DbModelFamilyBudgetEntities.IncomeCategory.ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new IncomeCategoryModel();
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IncomeCategoryModel incomeCategoryModel)
        {
            if (
                DbModelFamilyBudgetEntities.IncomeCategory.Any(
                    c => c.Name.Equals(incomeCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                         c.ID != incomeCategoryModel.Object.ID))
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.IncomeCategory.Add(incomeCategoryModel.Object);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    incomeCategoryModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", incomeCategoryModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(incomeCategoryModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IncomeCategory incomeCategory = DbModelFamilyBudgetEntities.IncomeCategory.Find(id);
            if (incomeCategory == null)
            {
                return HttpNotFound();
            }

            var model = new IncomeCategoryModel {Object = incomeCategory};
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IncomeCategoryModel incomeCategoryModel)
        {
            if (
                DbModelFamilyBudgetEntities.IncomeCategory.Any(
                    c => c.Name.Equals(incomeCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                         c.ID != incomeCategoryModel.Object.ID))
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.Entry(incomeCategoryModel.Object).State = EntityState.Modified;
                    DbModelFamilyBudgetEntities.SaveChanges();

                    incomeCategoryModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", incomeCategoryModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(incomeCategoryModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IncomeCategory incomeCategory = DbModelFamilyBudgetEntities.IncomeCategory.Find(id);
            if (incomeCategory == null)
            {
                return HttpNotFound();
            }

            var model = new IncomeCategoryModel {Object = incomeCategory};
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, IncomeCategoryModel incomeCategoryModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IncomeCategory incomeCategory = DbModelFamilyBudgetEntities.IncomeCategory.Find(id);
            if (incomeCategory == null)
            {
                return HttpNotFound();
            }

            if (DbModelFamilyBudgetEntities.Income.Any(e => e.CategoryID == incomeCategory.ID))
            {
                ModelState.AddModelError("",
                    "Эта категория имеет связанные с ней доходы. Сначала удалите связанные доходы");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.IncomeCategory.Remove(incomeCategory);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    incomeCategoryModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", incomeCategoryModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(incomeCategoryModel);
        }
    }
}