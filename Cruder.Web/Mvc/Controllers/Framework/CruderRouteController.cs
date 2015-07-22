using Cruder.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;

namespace Cruder.Web.Mvc.Controllers.Framework
{
    public class CruderRouteController : CruderWebController<Cruder.Data.Model.RouteEntity>
    {
        public CruderRouteController()
            : base(new CruderRouteRepository())
        {
        }

        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            ViewBag.HttpMethodSelectList = new List<SelectListItem>
            {
                new SelectListItem{Text = "GET", Value="get"},
                new SelectListItem{Text = "POST", Value="post"}
            };

            base.OnActionExecuting(filterContext);
        }

        public async Task<ActionResult> ControllerSync()
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<Type> types = new List<Type>();

            foreach (var item in assemblies)
            {
                var iterationTypes = item.GetTypes().Where(type => type.IsSubclassOf(typeof(System.Web.Mvc.Controller)) && !type.IsAbstract).ToList();

                if (iterationTypes.Count > 0)
                {
                    types.AddRange(iterationTypes);
                }
            }

            foreach (Type type in types)
            {
                string controllerName = type.Name.Replace("Controller", string.Empty);
                bool any = await Repository.Query(q => q.Controller.Equals(controllerName)).AnyAsync();

                if (!any)
                {
                    Cruder.Data.Model.RouteEntity route = new Data.Model.RouteEntity
                    {
                        Controller = controllerName
                    };

                    await Repository.SaveAsync(route, Cruder.Core.ActionType.Create);
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
        }
    }
}
