using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Auth.Infrastructure;
using Auth.Models;
using Auth.Repository;
using Auth.Repository.Interfaces;
using Auth.Services;
using Auth.Services.Interfaces;
using Auth.UnitOfWork;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;

namespace Auth
{
    public static class AutoFacContainer
    {
        public static IContainer Container { get; private set; }

        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ApplicationDbContext>().As<DbContext>();
            containerBuilder.RegisterType<ApplicationDbContext>().InstancePerRequest();
            containerBuilder.RegisterType<UnitOfWork.UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();


            //containerBuilder.RegisterType<UnitOfWork.UnitOfWork>().As<IUnitOfWork>();
            //            containerBuilder.RegisterType<AutoMapperProfile>().As<IMapper>();


            //containerBuilder.Register(c => new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); }))
            //    .AsSelf().SingleInstance();

            //containerBuilder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>()
            //    .InstancePerLifetimeScope();


            //containerBuilder.RegisterType<Mapper>().As<IMapper>();

            // ASP.NET identity

            containerBuilder.RegisterType<EmailService>().As<IIdentityMessageService>().InstancePerRequest();
            containerBuilder.RegisterType<ApplicationUserManager>().InstancePerRequest();
            containerBuilder.Register(c => Startup.DataProtectionProvider).InstancePerRequest();


            containerBuilder.Register(c => new UserStore<ApplicationUser>(c.Resolve<ApplicationDbContext>()))
                .AsImplementedInterfaces().InstancePerRequest();
            containerBuilder.Register(c => HttpContext.Current.GetOwinContext().Authentication)
                .As<IAuthenticationManager>();
            containerBuilder.Register(c => new IdentityFactoryOptions<ApplicationUserManager>
            {
                DataProtectionProvider = new DpapiDataProtectionProvider("ASP.NET Identity")
            });


            containerBuilder.RegisterType<UserStore<ApplicationUser>>().AsImplementedInterfaces().InstancePerRequest();


            RegisterRepositories(containerBuilder);
            RegisterServices(containerBuilder);
            RegisterMaps(containerBuilder);

            containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());


            Container = containerBuilder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
            Container.BeginLifetimeScope();
        }

        private static void RegisterServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AccountService>().As<IAccountService>();
        }

        private static void RegisterRepositories(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            // containerBuilder.RegisterType<UnitOfWork.UnitOfWork>().As<IUnitOfWork>();

            //containerBuilder.Register(ctx => ctx.Resolve<UnitOfWork.UnitOfWork>()).As<IUnitOfWork>();
        }

        /// <summary>
        ///     Registers the AutoMapper profile from the external assemblies.
        /// </summary>
        /// <param name="containerBuilder">The containerBuilder.</param>
        private static void RegisterMaps(ContainerBuilder containerBuilder)
        {
            var profiles =
                from t in typeof(AutoMapperProfile).Assembly.GetTypes()
                where typeof(Profile).IsAssignableFrom(t)
                select (Profile) Activator.CreateInstance(t);

            containerBuilder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles) cfg.AddProfile(profile);
            }));

            containerBuilder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();
        }
    }
}