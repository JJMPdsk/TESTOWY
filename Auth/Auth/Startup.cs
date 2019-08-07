using System.Web.Http;
using System.Web.Mvc;
using Auth;
using Auth.Models;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Auth
{
    public partial class Startup
    {
        public static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();
            // configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationUserManager>());

            UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>());

            var config = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(AutoFacContainer.Container)
            };
            app.UseAutofacMiddleware(AutoFacContainer.Container);
            app.UseWebApi(config);

            ConfigureAuth(app);
        }
    }
}