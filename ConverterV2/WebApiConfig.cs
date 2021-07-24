using System.Web.Http;

namespace ConverterV2
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("DefaultApi", "api/{controller}",
                new { id = RouteParameter.Optional });
        }
    }
}