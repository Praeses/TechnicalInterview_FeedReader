namespace FeedReader
{
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http;

    using FeedReader.Class;

    using Newtonsoft.Json.Serialization;

    public static class WebApiConfig
    {
        #region Public Methods and Operators

        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            GlobalConfiguration.Configuration.Filters.Add(new GlobalExceptionFilterAttribute());

            JsonMediaTypeFormatter jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        #endregion
    }
}