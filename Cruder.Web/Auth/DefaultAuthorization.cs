using System.Web.Mvc;
using System.Web.Routing;

namespace Cruder.Web.Auth
{
    public class DefaultAuthorization : BaseAuthorization
    {
        public override string AuthenticationType
        {
            get { return "Cruder.DefaultAuthorization"; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            BaseAuthorization.LoginIfDevelopmentEnvironment();

            BaseAuthorization.RenewPrincipalIdentity();

            if (!IsAllowedAnonymousAccess(filterContext.Controller))
            {
                base.OnAuthorization(filterContext);
            }
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues.Add("action", Cruder.Core.Configuration.ConfigurationFactory.AuthorizationRoute.LoginAction);
            routeValues.Add("controller", Cruder.Core.Configuration.ConfigurationFactory.AuthorizationRoute.Controller);

            if (filterContext.RequestContext.HttpContext.Request.UrlReferrer != null)
            {
                routeValues.Add("returnUrl", filterContext.RequestContext.HttpContext.Request.UrlReferrer.PathAndQuery);
            }
            
            filterContext.Result = new RedirectToRouteResult(routeValues);
        }
    }
}
