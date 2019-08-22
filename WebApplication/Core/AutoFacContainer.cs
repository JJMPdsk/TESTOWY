using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMapper;
using Core.Infrastructure;
using Core.Services;
using Core.Services.Interfaces;
using Core.Utilities;
using Data;
using Data.Models;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Data.UnitOfWork;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using IContainer = Autofac.IContainer;

namespace Core
{
    public static class AutoFacContainer
    {
        public static IContainer Container { get; private set; }

        public static void Register(ContainerBuilder containerBuilder)
        {
            RegisterDatabase(containerBuilder);
            containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            RegisterIdentity(containerBuilder);
            RegisterRepositories(containerBuilder);
            RegisterServices(containerBuilder);
            RegisterMaps(containerBuilder);
            RegisterControllers(containerBuilder);

            Container = containerBuilder.Build();
            SetResolvers();
            Container.BeginLifetimeScope();
        }

        private static void SetResolvers()
        {
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }

        private static void RegisterDatabase(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ApplicationDbContext>().As<DbContext>();
            containerBuilder.RegisterType<ApplicationDbContext>().InstancePerRequest();
        }

        /// <summary>
        ///     Rejestracja ASP.NET Identity
        /// </summary>
        /// <param name="containerBuilder"></param>
        private static void RegisterIdentity(ContainerBuilder containerBuilder)
        {
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
        }

        private static void RegisterControllers(ContainerBuilder containerBuilder)
        {
            // Web API
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // MVC 5
            containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     Rejestrowanie serwisów
        /// </summary>
        /// <param name="containerBuilder"></param>
        private static void RegisterServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<AccountService>().As<IAccountService>();
        }

        /// <summary>
        ///     Rejestrowanie repozytoriów
        /// </summary>
        /// <param name="containerBuilder"></param>
        private static void RegisterRepositories(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
        }

        /// <summary>
        ///     Rejestruje profile Automappera
        /// </summary>
        /// <param name="containerBuilder"></param>
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