using Auth;
using Autofac;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Auth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            ConfigureAuth(app);
        }
    }
}