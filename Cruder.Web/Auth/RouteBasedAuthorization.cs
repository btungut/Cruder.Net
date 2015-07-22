using Cruder.Web.Security;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cruder.Web.Auth
{
    public class RouteBasedAuthorization : BaseAuthorization
    {
        public override string AuthenticationType
        {
            get { return "Cruder.RouteBasedAuthorization"; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            BaseAuthorization.LoginIfDevelopmentEnvironment();

            BaseAuthorization.RenewPrincipalIdentity();

            if (!IsAllowedAnonymousAccess(filterContext.Controller))
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string httpMethod = filterContext.RequestContext.HttpContext.Request.HttpMethod;

                bool isAuthorized = CruderPrincipal.Current.IsInRole(string.Format("{0}.{1}.{2}", controller, action, httpMethod));

                if (!isAuthorized)
                {
                    this.HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            throw new NotSupportedException(); //This method shouldn't use. Because OnAuthorization has been overridden.
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues.Add("action", Cruder.Core.Configuration.ConfigurationFactory.AuthorizationRoute.LoginAction);
            routeValues.Add("controller", Cruder.Core.Configuration.ConfigurationFactory.AuthorizationRoute.Controller);
            routeValues.Add("authorizationErrorCode", Cruder.Core.AuthorizationErrorCode.AccessLevel.GetHashCode());

            if (filterContext.RequestContext.HttpContext.Request.UrlReferrer != null)
            {
                routeValues.Add("returnUrl", filterContext.RequestContext.HttpContext.Request.UrlReferrer.PathAndQuery);
            }

            filterContext.Result = new RedirectToRouteResult(routeValues);
        }
    }
}

