using Cruder.Data.Model;
using Cruder.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
namespace Cruder.Web.Mvc.Controllers.Framework
{
    public class CruderUserGroupController : CruderWebController<UserGroupEntity>
    {
        private readonly CruderRouteRepository routeRepository = null;

        public CruderUserGroupController()
            : base(new CruderUserGroupRepository())
        {
            routeRepository = new CruderRouteRepository();
        }

        public override async System.Threading.Tasks.Task<ActionResult> AddEdit()
        {
            var assignedRoutes = (await GetCurrentEntityAsync()).Routes;

            var routes = (await routeRepository.Query(null, q => q.OrderBy(x => x.Controller).ThenBy(x => x.Action).ThenBy(x => x.HttpMethod)).ToListAsync()).Select(route => new SelectListItem
            {
                Value = route.Id.ToString(),
                Text = string.Format("{0} / {1}  -  (HTTP {2})", GetString(route.Controller), GetString(route.Action), GetString(route.HttpMethod)),
                Selected = assignedRoutes.Any(assigned => assigned.Id == route.Id)
            }).ToList();

            ViewBag.Routes = routes;

            return await base.AddEdit();
        }

        protected override async System.Threading.Tasks.Task OnSaveExecutedAsync(UserGroupEntity entity, ViewModel.DetailViewModel<UserGroupEntity> viewModel, Model.ActionParameters parameters)
        {
            //Execute actions if UserGroup saving operation was successfully completed
            if (!parameters.OperationResult.HasError)
            {
                var assignedRoutes = (await GetCurrentEntityAsync()).Routes;
                var assignedIds = assignedRoutes.Select(q => q.Id).ToList();

                var selectedIds = new List<int>();
                if (Request.Form["External.Routes"] != null)
                {
                    selectedIds = Request.Form["External.Routes"].Split(',').Select(q => Convert.ToInt32(q)).ToList();
                }

                var willBeAddedIds = selectedIds.Except(assignedIds).ToList();

                var willBeDeletedIds = assignedIds.Except(selectedIds).ToList();

                foreach (var routeId in willBeAddedIds)
                {
                    (await GetCurrentEntityAsync()).RouteMappings.Add(new UserGroupRouteMappingEntity { RouteId = routeId, UserGroupId = entity.Id });
                }

                foreach (var routeId in willBeDeletedIds)
                {
                    var mapping = (await GetCurrentEntityAsync()).RouteMappings.Single(q => q.RouteId == routeId);
                    (await GetCurrentEntityAsync()).RouteMappings.Remove(mapping);
                }

                parameters.OperationResult = await Repository.SaveAsync(entity, Cruder.Core.ActionType.Update);
            }
        }

        private string GetString(string str)
        {
            return string.IsNullOrEmpty(str) ? "NULL" : str.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
            routeRepository.Dispose();
        }
    }
}
