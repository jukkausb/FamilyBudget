using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Areas.Administration.Models;
using FamilyBudget.v3.Controllers;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FamilyBudget.v3.Areas.Administration.Controllers
{
    public class InvestmentInstrumentTypeController : BaseController
    {
        private readonly IInvestmentInstrumentTypeRepository _investmentInstrumentTypeRepository;
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;

        public InvestmentInstrumentTypeController(IInvestmentInstrumentTypeRepository investmentInstrumentTypeRepository,
            IInvestmentInstrumentRepository investmentInstrumentRepository)
        {
            _investmentInstrumentTypeRepository = investmentInstrumentTypeRepository;
            _investmentInstrumentRepository = investmentInstrumentRepository;
        }

        public ViewResult Index(int? page, InvestmentInstrumentTypeListModel listModel)
        {
            try
            {
                listModel.PageSize = 100;
                listModel.ParseModelState(Request);
                listModel.Entities = _investmentInstrumentTypeRepository.GetAll().ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new InvestmentInstrumentTypeModel();
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvestmentInstrumentTypeModel investmentInstrumentTypeModel)
        {
            if (
                _investmentInstrumentTypeRepository.FindBy(
                    c =>
                        c.Code.Equals(investmentInstrumentTypeModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentTypeModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Тип для инструмента с таким кодом уже существует");
            }

            if (
                _investmentInstrumentTypeRepository.FindBy(
                    c =>
                        c.Name.Equals(investmentInstrumentTypeModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentTypeModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Тип для инструмента с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentTypeRepository.Add(investmentInstrumentTypeModel.Object);
                    _investmentInstrumentTypeRepository.SaveChanges();
                    investmentInstrumentTypeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentTypeModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentTypeModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvestmentInstrumentType investmentInstrumentType = _investmentInstrumentTypeRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrumentType == null)
            {
                return HttpNotFound();
            }

            var model = new InvestmentInstrumentTypeModel();
            model.Object = investmentInstrumentType;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InvestmentInstrumentTypeModel investmentInstrumentTypeModel)
        {
            if (
                _investmentInstrumentTypeRepository.FindBy(
                    c =>
                        c.Code.Equals(investmentInstrumentTypeModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentTypeModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Тип для инструмента с таким кодом уже существует");
            }

            if (
                _investmentInstrumentTypeRepository.FindBy(
                    c =>
                        c.Name.Equals(investmentInstrumentTypeModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentTypeModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Тип для инструмента с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentTypeRepository.Edit(investmentInstrumentTypeModel.Object);
                    _investmentInstrumentTypeRepository.SaveChanges();

                    investmentInstrumentTypeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentTypeModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentTypeModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvestmentInstrumentType investmentInstrumentType = _investmentInstrumentTypeRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrumentType == null)
            {
                return HttpNotFound();
            }

            var model = new InvestmentInstrumentTypeModel();
            model.Object = investmentInstrumentType;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, InvestmentInstrumentTypeModel investmentInstrumentTypeModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvestmentInstrumentType investmentInstrumentType = _investmentInstrumentTypeRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrumentType == null)
            {
                return HttpNotFound();
            }

            if (_investmentInstrumentRepository.FindBy(e => e.TypeID == investmentInstrumentType.ID).Any())
            {
                ModelState.AddModelError("",
                    "Этот тип имеет связанные с ней инструменты. Сначала удалите связанные инструменты");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentTypeRepository.Delete(investmentInstrumentType);
                    _investmentInstrumentTypeRepository.SaveChanges();
                    investmentInstrumentTypeModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentTypeModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentTypeModel);
        }
    }
}