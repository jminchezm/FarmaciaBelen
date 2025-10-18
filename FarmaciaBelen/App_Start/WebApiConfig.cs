using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FarmaciaBelen.App_Start
{
    public class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // api login
            config.MapHttpAttributeRoutes();

            // Ruta por defecto opcional
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //api empleados
            // JSON camelCase
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}