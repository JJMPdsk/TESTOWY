using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Auth.Controllers;
using Auth.Infrastructure;
using Autofac;
using Autofac.Integration.Mvc;

namespace Auth
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AutoFacContainer.Register(new ContainerBuilder());

       

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}