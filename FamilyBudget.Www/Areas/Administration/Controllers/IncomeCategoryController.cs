using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.App_Helpers;
using FamilyBudget.Www.Areas.Administration.Models;
using FamilyBudget.Www.Controllers;
using FamilyBudget.Www.Models.Repository.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FamilyBudget.Www.Areas.Administration.Controllers
{
    public class IncomeCategoryController : BaseController
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IIncomeCategoryRepository _incomeCategoryRepository;

        public IncomeCategoryController(IIncomeRepository incomeRepository, IIncomeCategoryRepository incomeCategoryRepository)
        {
            _incomeRepository = incomeRepository;
            _incomeCategoryRepository = incomeCategoryRepository;
        }

        public ViewResult Index(int? page, IncomeCategoryListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = _incomeCategoryRepository.GetAll().ToList();
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
                _incomeCategoryRepository.FindBy(
                    c => c.Name.Equals(incomeCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                         c.ID != incomeCategoryModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _incomeCategoryRepository.Add(incomeCategoryModel.Object);
                    _incomeCategoryRepository.SaveChanges();
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

            IncomeCategory incomeCategory = _incomeCategoryRepository.FindBy(ic => ic.ID == id.Value).FirstOrDefault();
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
                _incomeCategoryRepository.FindBy(
                    c => c.Name.Equals(incomeCategoryModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                         c.ID != incomeCategoryModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Категория с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _incomeCategoryRepository.Edit(incomeCategoryModel.Object);
                    _incomeCategoryRepository.SaveChanges();

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
            IncomeCategory incomeCategory = _incomeCategoryRepository.FindBy(ic => ic.ID == id.Value).FirstOrDefault();
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

            IncomeCategory incomeCategory = _incomeCategoryRepository.FindBy(ic => ic.ID == id.Value).FirstOrDefault();
            if (incomeCategory == null)
            {
                return HttpNotFound();
            }

            if (_incomeRepository.FindBy(e => e.CategoryID == incomeCategory.ID).Any())
            {
                ModelState.AddModelError("",
                    "Эта категория имеет связанные с ней доходы. Сначала удалите связанные доходы");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _incomeCategoryRepository.Delete(incomeCategory);
                    _incomeCategoryRepository.SaveChanges();
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