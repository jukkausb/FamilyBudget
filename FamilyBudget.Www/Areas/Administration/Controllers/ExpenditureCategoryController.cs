using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Areas.Administration.Models;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Repository.Interfaces;

namespace FamilyBudget.Www.Areas.Administration.Controllers
{
    public class ExpenditureCategoryController : BaseController
    {
        private IExpenditureRepository _expenditureRepository;
        private IExpenditureCategoryRepository _expenditureCategoryRepository;

        public ExpenditureCategoryController(IExpenditureRepository expenditureRepository, IExpenditureCategoryRepository expenditureCategoryRepository)
        {
            _expenditureRepository = expenditureRepository;
            _expenditureCategoryRepository = expenditureCategoryRepository;
        }

        public ViewResult Index(int? page, ExpenditureCategoryListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = _expenditureCategoryRepository.GetAll().ToList();
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
                _expenditureCategoryRepository.FindBy(
                    c =>
                        c.Name.Equals(ExpenditureCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != ExpenditureCategoryModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _expenditureCategoryRepository.Add(ExpenditureCategoryModel.Object);
                    _expenditureCategoryRepository.SaveChanges();
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

            ExpenditureCategory expenditureCategory = _expenditureCategoryRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (expenditureCategory == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureCategoryModel();
            model.Object = expenditureCategory;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExpenditureCategoryModel ExpenditureCategoryModel)
        {
            if (
                _expenditureCategoryRepository.FindBy(
                    c =>
                        c.Name.Equals(ExpenditureCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != ExpenditureCategoryModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _expenditureCategoryRepository.Edit(ExpenditureCategoryModel.Object);
                    _expenditureCategoryRepository.SaveChanges();

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
            ExpenditureCategory expenditureCategory = _expenditureCategoryRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (expenditureCategory == null)
            {
                return HttpNotFound();
            }

            var model = new ExpenditureCategoryModel();
            model.Object = expenditureCategory;
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

            ExpenditureCategory expenditureCategory = _expenditureCategoryRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (expenditureCategory == null)
            {
                return HttpNotFound();
            }

            if (_expenditureRepository.FindBy(e => e.CategoryID == expenditureCategory.ID).Any())
            {
                ModelState.AddModelError("",
                    "Эта категория имеет связанные с ней расходы. Сначала удалите связанные расходы");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _expenditureCategoryRepository.Delete(expenditureCategory);
                    _expenditureCategoryRepository.SaveChanges();
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