using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderRouteRepository : BaseCruderRepository<Cruder.Data.Model.RouteEntity>
    {
        public override System.Func<System.Linq.IQueryable<Cruder.Data.Model.RouteEntity>, System.Linq.IOrderedQueryable<Cruder.Data.Model.RouteEntity>> DefaultSorter
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
