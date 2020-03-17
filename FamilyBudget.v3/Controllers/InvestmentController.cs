using FamilyBudget.v3.App_CodeBase.Tinkoff;
using FamilyBudget.v3.Models;
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

            model.Accounts = await _tinkoffInvestmentDataProvider.GetInvestmentAccounts();

            return View(model);
        }
    }
}