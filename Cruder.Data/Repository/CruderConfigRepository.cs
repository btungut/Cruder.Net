using System;
using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderConfigRepository : BaseCruderRepository<Model.ConfigEntity, int>
    {
        public override Func<IQueryable<Model.ConfigEntity>, IOrderedQueryable<Model.ConfigEntity>> DefaultSorter
        {
            get
            {
                return q => q.OrderByDescending(x => x.UpdatedOn);
            }
        }

        protected override void OnSaveExecuting(Model.ConfigEntity entity, Core.ActionType actionType, ActionParameters parameters)
        {
            if (Queryable.Any(q => q.Key == entity.Key && q.Id != entity.Id))
            {
                parameters.OperationResult = CreateExistRecordResult(entity.Key);
            }
        }
    }
}
