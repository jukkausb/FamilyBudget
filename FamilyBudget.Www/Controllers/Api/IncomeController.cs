using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using FamilyBudget.Www.Models.Repository.Interfaces;
using System.Web.Http;

namespace FamilyBudget.Www.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/income")]
    public class IncomeController : ApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IIncomeCategoryRepository _incomeCategoryRepository;

        public IncomeController(IAccountRepository acountRepository, IIncomeRepository incomeRepository, IIncomeCategoryRepository incomeCategoryRepository)
        {
            _accountRepository = acountRepository;
            _incomeRepository = incomeRepository;
            _incomeCategoryRepository = incomeCategoryRepository;
        }

        [HttpGet] //change to [Route("availability")] after upgrading to mvc5
        public HttpResponseMessage Get()
        {
            Thread.Sleep(3000);
            return Request.CreateResponse(HttpStatusCode.OK);
            //throw new Exception("test api exception");
        }
    }
}
