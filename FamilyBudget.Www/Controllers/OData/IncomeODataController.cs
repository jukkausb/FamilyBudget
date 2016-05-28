using System.Collections.Generic;
using System.Linq;
using FamilyBudget.Www.App_DataModel;
using FamilyBudget.Www.Models.Repository.Interfaces;
using Microsoft.Data.OData;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace FamilyBudget.Www.Controllers.OData
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using FamilyBudget.Www.App_DataModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Income>("IncomeOData");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class IncomeODataController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IIncomeCategoryRepository _incomeCategoryRepository;

        public IncomeODataController(IAccountRepository acountRepository, IIncomeRepository incomeRepository, IIncomeCategoryRepository incomeCategoryRepository)
        {
            _accountRepository = acountRepository;
            _incomeRepository = incomeRepository;
            _incomeCategoryRepository = incomeCategoryRepository;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<Income> Get()
        {
            return _incomeRepository.Context.Income.AsQueryable();
        }

        //// GET: odata/IncomeOData
        //[EnableQuery]
        //public IHttpActionResult GetIncomeOData(ODataQueryOptions<Income> queryOptions)
        //{
        //    // validate the query.
        //    try
        //    {
        //        queryOptions.Validate(_validationSettings);
        //    }
        //    catch (ODataException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    IQueryable<Income> query = _incomeRepository.Context.Income.AsQueryable();
        //    return Ok<IEnumerable<Income>>(query.ToList());
        //    //return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //// GET: odata/IncomeOData(5)
        //public IHttpActionResult GetIncome([FromODataUri] int key, ODataQueryOptions<Income> queryOptions)
        //{
        //    // validate the query.
        //    try
        //    {
        //        queryOptions.Validate(_validationSettings);
        //    }
        //    catch (ODataException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    // return Ok<Income>(income);
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //// PUT: odata/IncomeOData(5)
        //public IHttpActionResult Put([FromODataUri] int key, Delta<Income> delta)
        //{
        //    Validate(delta.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // TODO: Get the entity here.

        //    // delta.Put(income);

        //    // TODO: Save the patched entity.

        //    // return Updated(income);
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //// POST: odata/IncomeOData
        //public IHttpActionResult Post(Income income)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // TODO: Add create logic here.

        //    // return Created(income);
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //// PATCH: odata/IncomeOData(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public IHttpActionResult Patch([FromODataUri] int key, Delta<Income> delta)
        //{
        //    Validate(delta.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // TODO: Get the entity here.

        //    // delta.Patch(income);

        //    // TODO: Save the patched entity.

        //    // return Updated(income);
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //// DELETE: odata/IncomeOData(5)
        //public IHttpActionResult Delete([FromODataUri] int key)
        //{
        //    // TODO: Add delete logic here.

        //    // return StatusCode(HttpStatusCode.NoContent);
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}
    }
}
