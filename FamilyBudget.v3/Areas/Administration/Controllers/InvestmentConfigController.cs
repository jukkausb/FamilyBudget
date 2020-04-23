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
    public class InvestmentConfigController : BaseController
    {
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;

        public InvestmentConfigController(IInvestmentInstrumentRepository investmentInstrumentRepository)
        {
            _investmentInstrumentRepository = investmentInstrumentRepository;
        }

        public ViewResult Index()
        {
            try
            {
                InvestmentConfigModel model = new InvestmentConfigModel();

                model.InvestmentInstrumentListModel = new InvestmentInstrumentListModel
                {
                    Entities = _investmentInstrumentRepository.GetAll().ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}