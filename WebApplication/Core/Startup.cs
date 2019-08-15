using System.Web.Mvc;
using Core;
using Data;
using Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Core
{
    public partial class Startup
    {
        public static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            // configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationUserManager>());

            UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>());

            app.UseAutofacMiddleware(AutoFacContainer.Container);

            ConfigureAuth(app);
        }
    }
}