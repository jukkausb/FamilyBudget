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
    public class CurrencyController : BaseController
    {
        private ICurrencyRepository _currencyRepository;
        private IAccountRepository _accountRepository;

        public CurrencyController(ICurrencyRepository currencyRepository, IAccountRepository accountRepository)
        {
            _currencyRepository = currencyRepository;
            _accountRepository = accountRepository;
        }

        public ViewResult Index(int? page, CurrencyListModel listModel)
        {
            try
            {
                listModel.ParseModelState(Request);
                listModel.Entities = _currencyRepository.GetAll().ToList();
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
            if (_currencyRepository.FindBy(c => c.Code.Equals(CurrencyModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) && c.ID != CurrencyModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Валюта с таким кодом уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _currencyRepository.Add(CurrencyModel.Object);
                    _currencyRepository.SaveChanges();
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

            Currency currency = _currencyRepository.FindBy(c => c.ID == id.Value).FirstOrDefault();
            if (currency == null)
            {
                return HttpNotFound();
            }

            var model = new CurrencyModel();
            model.Object = currency;
            model.RestoreModelState(Request.QueryString[QueryStringParser.GridReturnParameters]);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CurrencyModel currencyModel)
        {
            if (
                _currencyRepository.FindBy(c => c.Code.Equals(currencyModel.Object.Code, StringComparison.InvariantCultureIgnoreCase) && c.ID != currencyModel.Object.ID).Any())
            {
                ModelState.AddModelError("", "Валюта с таким кодом уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _currencyRepository.Edit(currencyModel.Object);
                    _currencyRepository.SaveChanges();

                    currencyModel.RestoreModelState(Request[QueryStringParser.GridReturnParameters]);
                    return RedirectToAction("Index", currencyModel.ToRouteValueDictionary());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }

            return View(currencyModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = _currencyRepository.FindBy(c => c.ID == id.Value).FirstOrDefault();
            if (currency == null)
            {
                return HttpNotFound();
            }

            var model = new CurrencyModel();
            model.Object = currency;
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

            Currency currency = _currencyRepository.FindBy(c => c.ID == id.Value).FirstOrDefault();
            if (currency == null)
            {
                return HttpNotFound();
            }

            if (_accountRepository.FindBy(e => e.CurrencyID == currency.ID).Any())
            {
                ModelState.AddModelError("", "Эта валюта имеет связанные с ней счета. Сначала удалите связанные счета");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _currencyRepository.Delete(currency);
                    _currencyRepository.SaveChanges();
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