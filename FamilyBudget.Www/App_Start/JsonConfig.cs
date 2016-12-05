using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace FamilyBudget.Www.App_Start
{
    public class JsonConfig
    {
        public static void SetSerializerSettings(HttpConfiguration config)
        {
            config.Formatters.RemoveAt(0);
            config.Formatters.Insert(0, new PascalCaseJsonFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new DecimalFormatConverter());
        }
    }

    public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal));
        }

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            writer.WriteValue(((decimal)value).ToString("N2", new CultureInfo("ru-RU")));
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                     object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class PascalCaseJsonFormatter : JsonMediaTypeFormatter
    {
        private IHttpRouteData _route;
        private readonly IContractResolver _defaultResolver = new DefaultContractResolver();
        private readonly IContractResolver _camelCaseResolver = new CamelCasePropertyNamesContractResolver();

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, System.Net.Http.Headers.MediaTypeHeaderValue mediaType)
        {
            _route = request.GetRouteData();
            return base.GetPerRequestFormatterInstance(type, request, mediaType);
        }

        public override System.Threading.Tasks.Task WriteToStreamAsync(Type type, object value, System.IO.Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            // EpiFind needs different formatting
            if (_route.Route.RouteTemplate.Contains("testui/Find"))
            {
                SerializerSettings.ContractResolver = _defaultResolver;
            }
            else
            {
                SerializerSettings.ContractResolver = _camelCaseResolver;
            }

            return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        }
    }
}