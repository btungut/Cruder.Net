using Cruder.Data.Model;
using System;
using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderConfigRepository : BaseCruderRepository<ConfigEntity>
    {
        public override Func<IQueryable<ConfigEntity>, IOrderedQueryable<ConfigEntity>> DefaultSorter
        {
            get
            {
                return q => q.OrderByDescending(x => x.UpdatedOn);
            }
        }

        protected override void OnSaveExecuting(ConfigEntity entity, Core.ActionType actionType, ActionParameters parameters)
        {
            if (Queryable.Any(q => q.Key == entity.Key && q.Id != entity.Id))
            {
                parameters.OperationResult = CreateExistRecordResult(entity.Key);
            }
        }
    }
}
