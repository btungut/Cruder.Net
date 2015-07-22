using Cruder.Core.Module;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using Cruder.Core;
using Cruder.Core.Contract;
using Cruder.Web.Model;
using System.Data.Entity;

namespace Cruder.Web
{
    public abstract class CruderHttpApplication : HttpApplication
    {
        public abstract InitializationSettings InitializationSettings { get; }

        private static readonly Predicate<Assembly> defaultAssemblyFilter = assembly =>
            !assembly.FullName.StartsWith("System.") &&
            !assembly.FullName.StartsWith("Microsoft.") &&
            !assembly.FullName.StartsWith("Newtonsoft.") &&
            !assembly.FullName.StartsWith("Castle.");

        protected virtual void Application_Start()
        {
            InitializeApp();

            RegisterRepositories();
            RegisterControllers();
            RegisterContainerImplementations();
            RegisterEntityFrameworkContexts();

            InitializeViewEngines();

            RegisterFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterEntityFrameworkContexts()
        {
            if (InitializationSettings.RegisterEntityFrameworkContexts)
            {
                string entityFrameworkNamespace = typeof(DbContext).Namespace;
                string cruderContextNamespace = typeof(Cruder.Data.CruderDbContext).Namespace;

                Castle.MicroKernel.Registration.BasedOnDescriptor registration = IoC.AssemblyDescriptor
                    .BasedOn<DbContext>()
                    .If(x => 
                        !x.Namespace.StartsWith(entityFrameworkNamespace) && 
                        !x.Namespace.StartsWith(cruderContextNamespace))
                    .WithServiceBase()
                    .LifestylePerWebRequest();

                IoC.Register(
                    registration
                    );
            }
        }

        private void RegisterContainerImplementations()
        {
            if (InitializationSettings.RegisterCustomContainerImplementations)
            {
                Castle.MicroKernel.Registration.BasedOnDescriptor registration = IoC.AssemblyDescriptor
                    .BasedOn<IContainerImplementation>()
                    .WithService
                    .AllInterfaces()
                    .LifestylePerWebRequest();

                IoC.Register(
                    registration
                    );
                }
        }

        protected virtual void InitializeApp()
        {
            Predicate<Assembly> combinedFilter = InitializationSettings.AssemblyFilter == null ? defaultAssemblyFilter : (Predicate<Assembly>)Delegate.Combine(InitializationSettings.AssemblyFilter, defaultAssemblyFilter);

            IoC.Bootstrap(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "bin"), combinedFilter);

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IoC.Container));

            Logger.Log(LogType.Info, Priority.None, string.Format("'{0}' has been initialized.", GetType().BaseType.Namespace), LogModule.Framework);
        }

        private void RegisterRepositories()
        {
            if (InitializationSettings.RepositoryRegistration != RepositoryRegistrationSetting.None)
            {
                Castle.MicroKernel.Registration.BasedOnDescriptor registration = null;

                if (InitializationSettings.RepositoryRegistration == RepositoryRegistrationSetting.All)
                {
                    registration = IoC.AssemblyDescriptor
                        .BasedOn(typeof(ICruderRepository<>))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest();
                }
                else if (InitializationSettings.RepositoryRegistration == RepositoryRegistrationSetting.OnlyAssemblyRepositories)
                {
                    registration = IoC.AssemblyDescriptor
                        .BasedOn(typeof(ICruderRepository<>))
                        .If(q => !q.Name.StartsWith("Cruder"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest();
                }
                else if (InitializationSettings.RepositoryRegistration == RepositoryRegistrationSetting.OnlyCruderRepositories)
                {
                    registration = IoC.AssemblyDescriptor
                        .BasedOn(typeof(ICruderRepository<>))
                        .If(q => q.Name.StartsWith("Cruder"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest();
                }

                IoC.Register(registration);
            }
        }

        private void RegisterControllers()
        {
            if (InitializationSettings.ControllerRegistration != ControllerRegistrationSetting.None)
            {
                Castle.MicroKernel.Registration.BasedOnDescriptor registration = null;

                if (InitializationSettings.ControllerRegistration == ControllerRegistrationSetting.All)
                {
                    registration = IoC.AssemblyDescriptor
                        .BasedOn<IController>()
                        .LifestylePerWebRequest();
                }
                else if (InitializationSettings.ControllerRegistration == ControllerRegistrationSetting.OnlyAssemblyControllers)
                {
                    registration = IoC.AssemblyDescriptor
                        .BasedOn<IController>()
                        .If(q => !q.Name.StartsWith("Cruder"))
                        .LifestylePerWebRequest();
                }
                else if (InitializationSettings.ControllerRegistration == ControllerRegistrationSetting.OnlyCruderControllers)
                {
                    registration = IoC.AssemblyDescriptor
                        .BasedOn<IController>()
                        .If(q => q.Name.StartsWith("Cruder"))
                        .LifestylePerWebRequest();
                }

                IoC.Register(registration);
            }
        }

        protected virtual void InitializeViewEngines()
        {
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().First();

            razorEngine.ViewLocationFormats = new string[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",

                "~/Views/_CruderViews/{1}/{0}.cshtml",
                "~/Views/_CruderViews/Shared/{0}.cshtml"
            };

            razorEngine.PartialViewLocationFormats = new string[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",

                "~/Views/_CruderViews/{1}/{0}.cshtml",
                "~/Views/_CruderViews/Shared/{0}.cshtml"
            };
        }

        protected virtual void RegisterFilters(GlobalFilterCollection filters)
        {
            if (InitializationSettings.AuthorizationFilter != null)
            {
                filters.Add(InitializationSettings.AuthorizationFilter);
            }
        }

        protected virtual void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.Ignore("{*js}", new { js = @".*\.js(/.*)?" });
            routes.Ignore("{*css}", new { css = @".*\.css(/.*)?" });
            routes.Ignore("{*png}", new { png = @".*\.png(/.*)?" });
            routes.Ignore("{*jpg}", new { jpg = @".*\.jpg(/.*)?" });
            routes.Ignore("{*gif}", new { gif = @".*\.gif(/.*)?" });
            routes.Ignore("{*favicon}", new { favicon = "(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected virtual void Application_End() { }
        protected virtual void Application_ReleaseRequestState() { }
        protected virtual void Application_BeginRequest() { }
        protected virtual void Session_Start() { }
        protected virtual void Session_End() { }
        protected virtual void Application_EndReques() { }
        protected virtual void Application_AcquireRequestState() { }
        protected virtual void Application_AuthenticateRequest() { }

        protected virtual void Application_Error()
        {
            ExceptionHandler.HandleLastError();
        }
    }
}
