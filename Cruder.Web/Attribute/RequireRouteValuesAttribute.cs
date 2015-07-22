using System.Reflection;
using System.Web.Mvc;

namespace Cruder.Web
{
    public class RequireRouteValuesAttribute : ActionMethodSelectorAttribute
    {
        public string[] Parameters { get; private set; }

        public RequireRouteValuesAttribute(string[] parameters)
        {
            Parameters = parameters;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            bool contains = false;
            foreach (string parameter in Parameters)
            {
                contains = controllerContext.RequestContext.RouteData.Values.ContainsKey(parameter);
                if (!contains) break;
            }
            return contains;
        }
    }
}
