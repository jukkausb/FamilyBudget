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
    public class InvestmentInstrumentMarketController : BaseController
    {
        private readonly IInvestmentInstrumentMarketRepository _investmentInstrumentMarketRepository;
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;

        public InvestmentInstrumentMarketController(IInvestmentInstrumentMarketRepository investmentInstrumentMarketRepository,
            IInvestmentInstrumentRepository investmentInstrumentRepository)
        {
            _investmentInstrumentMarketRepository = investmentInstrumentMarketRepository;
            _investmentInstrumentRepository = investmentInstrumentRepository;
        }

        public ViewResult Index(int? page, InvestmentInstrumentMarketListModel listModel)
        {
            try
            {
                listModel.PageSize = 100;
                listModel.ParseModelState(Request);
                listModel.Entities = _investmentInstrumentMarketRepository.GetAll().ToList();
                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new InvestmentInstrumentMarketModel();
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvestmentInstrumentMarketModel investmentInstrumentMarketModel)
        {
            if (
                _investmentInstrumentMarketRepository.FindBy(
                    c =>
                        c.Code.Equals(investmentInstrumentMarketModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentMarketModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Рынок для инструмента с таким кодом уже существует");
            }

            if (
                _investmentInstrumentMarketRepository.FindBy(
                    c =>
                        c.Name.Equals(investmentInstrumentMarketModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentMarketModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Рынок для инструмента с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentMarketRepository.Add(investmentInstrumentMarketModel.Object);
                    _investmentInstrumentMarketRepository.SaveChanges();
                    investmentInstrumentMarketModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentMarketModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentMarketModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvestmentInstrumentMarket investmentInstrumentMarket = _investmentInstrumentMarketRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrumentMarket == null)
            {
                return HttpNotFound();
            }

            var model = new InvestmentInstrumentMarketModel();
            model.Object = investmentInstrumentMarket;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InvestmentInstrumentMarketModel investmentInstrumentMarketModel)
        {
            if (
                _investmentInstrumentMarketRepository.FindBy(
                    c =>
                        c.Code.Equals(investmentInstrumentMarketModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentMarketModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Рынок для инструмента с таким кодом уже существует");
            }

            if (
                _investmentInstrumentMarketRepository.FindBy(
                    c =>
                        c.Name.Equals(investmentInstrumentMarketModel.Object.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentMarketModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Рынок для инструмента с таким именем уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentMarketRepository.Edit(investmentInstrumentMarketModel.Object);
                    _investmentInstrumentMarketRepository.SaveChanges();

                    investmentInstrumentMarketModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentMarketModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentMarketModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvestmentInstrumentMarket investmentInstrumentMarket = _investmentInstrumentMarketRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrumentMarket == null)
            {
                return HttpNotFound();
            }

            var model = new InvestmentInstrumentMarketModel();
            model.Object = investmentInstrumentMarket;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, InvestmentInstrumentMarketModel investmentInstrumentMarketModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvestmentInstrumentMarket investmentInstrumentMarket = _investmentInstrumentMarketRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrumentMarket == null)
            {
                return HttpNotFound();
            }

            if (_investmentInstrumentRepository.FindBy(e => e.MarketID == investmentInstrumentMarket.ID).Any())
            {
                ModelState.AddModelError("",
                    "Этот рынок имеет связанные с ней инструменты. Сначала удалите связанные инструменты");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentMarketRepository.Delete(investmentInstrumentMarket);
                    _investmentInstrumentMarketRepository.SaveChanges();
                    investmentInstrumentMarketModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentMarketModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentMarketModel);
        }
    }
}