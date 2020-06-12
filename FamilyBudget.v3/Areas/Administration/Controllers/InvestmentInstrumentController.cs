using FamilyBudget.v3.App_DataModel;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Areas.Administration.Models;
using FamilyBudget.v3.Controllers;
using FamilyBudget.v3.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FamilyBudget.v3.Areas.Administration.Controllers
{
    public class InvestmentInstrumentController : BaseController
    {
        private readonly IInvestmentInstrumentTypeRepository _investmentInstrumentTypeRepository;
        private readonly IInvestmentInstrumentMarketRepository _investmentInstrumentMarketRepository;
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;

        public InvestmentInstrumentController(IInvestmentInstrumentRepository investmentInstrumentRepository,
            IInvestmentInstrumentTypeRepository investmentInstrumentTypeRepository, 
            IInvestmentInstrumentMarketRepository investmentInstrumentMarketRepository)
        {
            _investmentInstrumentRepository = investmentInstrumentRepository;
            _investmentInstrumentTypeRepository = investmentInstrumentTypeRepository;
            _investmentInstrumentMarketRepository = investmentInstrumentMarketRepository;
        }

        public ViewResult Index(int? page, InvestmentInstrumentListModel listModel)
        {
            try
            {
                var instruments = _investmentInstrumentRepository.GetAll().ToList();
                var entities = instruments.Select(i => new InvestmentInstrumentInfoModel
                {
                    ID = i.ID,
                    Code = i.Code,
                    DiagramBackgroundColor = i.DiagramBackgroundColor,
                    DiagramBackgroundHoverColor = i.DiagramBackgroundHoverColor,
                    DiagramHoverBorderColor = i.DiagramHoverBorderColor,
                    PortfolioPercent = i.PortfolioPercent,
                    PortfolioPercentDelta = i.PortfolioPercentDelta,
                    ExternalAvatarIsinOverride = i.ExternalAvatarIsinOverride,
                    ExternalPageTickerOverride = i.ExternalPageTickerOverride,
                    Type = i.InvestmentInstrumentType?.Name,
                    Market = i.InvestmentInstrumentMarket?.Name
                }).ToList();

                listModel.PageSize = 1000;
                listModel.ParseModelState(Request);
                listModel.Entities = entities;

                return View(listModel);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public ActionResult Create()
        {
            var model = new InvestmentInstrumentModel()
            { 
                InvestmentInstrumentTypes = GetInvestmentInstrumentTypes(),
                InvestmentInstrumentMarkets = GetInvestmentInstrumentMarkets()
            };
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvestmentInstrumentModel investmentInstrumentModel)
        {
            if (
                _investmentInstrumentRepository.FindBy(
                    c =>
                        c.Code.Equals(investmentInstrumentModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Инструмент с таким кодом уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentRepository.Add(investmentInstrumentModel.Object);
                    _investmentInstrumentRepository.SaveChanges();
                    investmentInstrumentModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            else 
            {
                investmentInstrumentModel.InvestmentInstrumentTypes = GetInvestmentInstrumentTypes();
                investmentInstrumentModel.InvestmentInstrumentMarkets = GetInvestmentInstrumentMarkets();
            }

            return View(investmentInstrumentModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvestmentInstrument investmentInstrument = _investmentInstrumentRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrument == null)
            {
                return HttpNotFound();
            }

            var model = new InvestmentInstrumentModel()
            {
                InvestmentInstrumentTypes = GetInvestmentInstrumentTypes(),
                InvestmentInstrumentMarkets = GetInvestmentInstrumentMarkets()
            };
            model.Object = investmentInstrument;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InvestmentInstrumentModel investmentInstrumentModel)
        {
            if (
                _investmentInstrumentRepository.FindBy(
                    c =>
                        c.Code.Equals(investmentInstrumentModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) &&
                        c.ID != investmentInstrumentModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Инструмент с таким кодом уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentRepository.Edit(investmentInstrumentModel.Object);
                    _investmentInstrumentRepository.SaveChanges();

                    investmentInstrumentModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            else
            {
                investmentInstrumentModel.InvestmentInstrumentTypes = GetInvestmentInstrumentTypes();
                investmentInstrumentModel.InvestmentInstrumentMarkets = GetInvestmentInstrumentMarkets();
            }


            return View(investmentInstrumentModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvestmentInstrument investmentInstrument = _investmentInstrumentRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrument == null)
            {
                return HttpNotFound();
            }

            var model = new InvestmentInstrumentModel();
            model.Object = investmentInstrument;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, InvestmentInstrumentModel investmentInstrumentModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvestmentInstrument investmentInstrument = _investmentInstrumentRepository.FindBy(ec => ec.ID == id).FirstOrDefault();
            if (investmentInstrument == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _investmentInstrumentRepository.Delete(investmentInstrument);
                    _investmentInstrumentRepository.SaveChanges();
                    investmentInstrumentModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", investmentInstrumentModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(investmentInstrumentModel);
        }

        protected List<SelectListItem> GetInvestmentInstrumentTypes()
        {
            List<SelectListItem> currencies =
                _investmentInstrumentTypeRepository.GetAll().ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0} ({1})", c.Name, c.Code)
                }).ToList();

            currencies.Insert(0, new SelectListItem { Text = " - Выберите тип инструмента - ", Value = "" });
            return currencies;
        }

        protected List<SelectListItem> GetInvestmentInstrumentMarkets()
        {
            List<SelectListItem> currencies =
                _investmentInstrumentMarketRepository.GetAll().ToList().Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(CultureInfo.InvariantCulture),
                    Text = string.Format("{0} ({1})", c.Name, c.Code)
                }).ToList();

            currencies.Insert(0, new SelectListItem { Text = " - Выберите рынок инструмента - ", Value = "" });
            return currencies;
        }
    }
}