using System;
using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderUserGroupRepository : BaseCruderRepository<Cruder.Data.Model.UserGroupEntity>
    {
        public override Func<IQueryable<Model.UserGroupEntity>, IOrderedQueryable<Model.UserGroupEntity>> DefaultSorter
        {
            get
            {
                return q => q.OrderByDescending(x => x.UpdatedOn);
            }
        }

        protected override void OnSaveExecuting(Model.UserGroupEntity entity, Core.ActionType actionType, ActionParameters parameters)
        {
            if (Queryable.Any(q => q.Name == entity.Name && q.Id != entity.Id))
            {
                parameters.OperationResult = CreateExistRecordResult(entity.Name);
            }
        }
    }
}
