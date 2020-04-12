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
        private readonly IInvestmentRulesEtfRepository _investmentRulesEtfRepository;
        private readonly IInvestmentRulesInstrumentsRepository _investmentRulesInstrumentsRepository;

        public InvestmentConfigController(IInvestmentRulesEtfRepository investmentRulesEtfRepository,
            IInvestmentRulesInstrumentsRepository investmentRulesInstrumentsRepository)
        {
            _investmentRulesEtfRepository = investmentRulesEtfRepository;
            _investmentRulesInstrumentsRepository = investmentRulesInstrumentsRepository;
        }

        public ViewResult Index()
        {
            try
            {
                InvestmentConfigModel model = new InvestmentConfigModel();

                model.InvestmentRulesEtfListModel = new InvestmentRulesEtfListModel
                {
                    Entities = _investmentRulesEtfRepository.GetAll().ToList()
                };
                model.InvestmentRulesInstrumentsListModel = new InvestmentRulesInstrumentsListModel
                {
                    Entities = _investmentRulesInstrumentsRepository.GetAll().ToList()
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