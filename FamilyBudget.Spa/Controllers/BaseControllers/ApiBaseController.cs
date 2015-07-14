using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FamilyBudget.Spa.Controllers.BaseControllers
{
    public class ApiBaseController : ApiController
    {
        private JsonSerializerSettings JsonLoggingSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver(),
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                };
            }
        }

        //protected void LogRequestFromClient(string message, object model)
        //{
        //    try
        //    {
        //        if (PropertyRetriever.LogWebapiRequests)
        //        {
        //            string modelAsJsonString = string.Empty;
        //            if (model != null)
        //            {
        //                modelAsJsonString = JsonConvert.SerializeObject(model, Formatting.None, JsonLoggingSettings);
        //            }
        //            NasLog.Info(string.Format("{0}, {1}", message, modelAsJsonString));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NasLog.Warn("ApiBaseController.LogRequestFromClient", ex);
        //    }
        //}
    }
}