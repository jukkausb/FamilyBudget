using FamilyBudget.v3.App_CodeBase.Tinkoff;
using FamilyBudget.v3.App_Utils;
using FamilyBudget.v3.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FamilyBudget.v3.Controllers
{
    public class InvestmentController : BaseController
    {
        private readonly ITinkoffInvestmentDataProvider _tinkoffInvestmentDataProvider;

        public InvestmentController(ITinkoffInvestmentDataProvider tinkoffInvestmentDataProvider)
        {
            _tinkoffInvestmentDataProvider = tinkoffInvestmentDataProvider;
        }

        public async Task<ActionResult> Index()
        {
            InvestmentModel model = new InvestmentModel();

            try
            {
                model.Accounts = await _tinkoffInvestmentDataProvider.GetInvestmentAccounts();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                model.Message = new MessageModel();
                model.Message.Messages.Add(new Message
                {
                    Text = "Ошибка загрузки данных с Tinkoff",
                    Type = MessageType.Error
                });
            }

            return View(model);
        }
    }
}