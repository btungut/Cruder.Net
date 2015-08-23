using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderRouteRepository : BaseCruderRepository<Model.RouteEntity, int>
    {
        public override System.Func<System.Linq.IQueryable<Model.RouteEntity>, System.Linq.IOrderedQueryable<Model.RouteEntity>> DefaultSorter
        {
            get
            {
                return q => q.OrderByDescending(x => x.UpdatedOn);
            }
        }

        protected override void OnSaveExecuting(Model.RouteEntity entity, Core.ActionType actionType, ActionParameters parameters)
        {
            if (string.IsNullOrEmpty(entity.HttpMethod))
            {
                entity.HttpMethod = null;
            }

            if (string.IsNullOrEmpty(entity.Action))
            {
                entity.Action = null;
            }
        }
    }
}
