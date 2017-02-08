using API.ActionFilters;
using API.Filters;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // cors
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            // formatters
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Custom filters - validate JSON post request
            config.Filters.Add(new ValidateModelAttribute());
            // Logger
            config.Filters.Add(new LoggingFilterAttribute());
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            config.Filters.Add(new GlobalExceptionAttribute());


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
