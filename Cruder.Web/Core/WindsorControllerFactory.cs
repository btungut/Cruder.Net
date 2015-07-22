using Castle.Windsor;
using Cruder.Web.Security;
using System;
using System.Web;
using System.Web.Mvc;

namespace Cruder.Web
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer container = null;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                if (CruderPrincipal.Current == null)
                {
                    var settings = (requestContext.HttpContext.ApplicationInstance as CruderHttpApplication).InitializationSettings;

                    CruderPrincipal.Current = new CruderPrincipal(
                    settings.AuthorizationFilter == null ? null : settings.AuthorizationFilter.AuthenticationType,
                    UserSessionManager.GetCurrent()
                    );
                }

                return (IController)container.Resolve(controllerType);
            }
            else
            {
                throw new HttpException(404, "Requested controller could not found.");
            }
        }

        public override void ReleaseController(IController controller)
        {
            container.Release(controller);
        }
    }
}
