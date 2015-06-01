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
    public class CurrencyController : BaseController
    {
        public ViewResult Index(int? page, CurrencyListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = DbModelFamilyBudgetEntities.Currency.ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new CurrencyModel();
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurrencyModel CurrencyModel)
        {
            if (
                DbModelFamilyBudgetEntities.Currency.Any(
                    c => c.Code.Equals(CurrencyModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                         c.ID != CurrencyModel.Object.ID))
            {
                ModelState.AddModelError("", "Валюта с таким кодом уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.Currency.Add(CurrencyModel.Object);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    CurrencyModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", CurrencyModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(CurrencyModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Currency Currency = DbModelFamilyBudgetEntities.Currency.Find(id);
            if (Currency == null)
            {
                return HttpNotFound();
            }

            var model = new CurrencyModel();
            model.Object = Currency;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CurrencyModel CurrencyModel)
        {
            if (
                DbModelFamilyBudgetEntities.Currency.Any(
                    c => c.Code.Equals(CurrencyModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                         c.ID != CurrencyModel.Object.ID))
            {
                ModelState.AddModelError("", "Валюта с таким кодом уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.Entry(CurrencyModel.Object).State = EntityState.Modified;
                    DbModelFamilyBudgetEntities.SaveChanges();

                    CurrencyModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", CurrencyModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(CurrencyModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency Currency = DbModelFamilyBudgetEntities.Currency.Find(id);
            if (Currency == null)
            {
                return HttpNotFound();
            }

            var model = new CurrencyModel();
            model.Object = Currency;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, CurrencyModel CurrencyModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Currency Currency = DbModelFamilyBudgetEntities.Currency.Find(id);
            if (Currency == null)
            {
                return HttpNotFound();
            }

            if (DbModelFamilyBudgetEntities.Account.Any(e => e.CurrencyID == Currency.ID))
            {
                ModelState.AddModelError("", "Эта валюта имеет связанные с ней счета. Сначала удалите связанные счета");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DbModelFamilyBudgetEntities.Currency.Remove(Currency);
                    DbModelFamilyBudgetEntities.SaveChanges();
                    CurrencyModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", CurrencyModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(CurrencyModel);
        }
    }
}