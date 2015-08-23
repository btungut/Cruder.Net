using System;
using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderUserRepository : BaseCruderRepository<Model.UserEntity,int>
    {
        public override Func<IQueryable<Model.UserEntity>, IOrderedQueryable<Model.UserEntity>> DefaultSorter
        {
            get
            {
                return q => q.OrderByDescending(x => x.UpdatedOn);
            }
        }

        protected override void OnSaveExecuting(Model.UserEntity entity, Core.ActionType actionType, ActionParameters parameters)
        {
            if (Queryable.Any(q => q.Username == entity.Username && q.Id != entity.Id))
            {
                parameters.OperationResult = CreateExistRecordResult(entity.Username);
            }
            else if (Queryable.Any(q => q.Mail == entity.Mail && q.Id != entity.Id))
            {
                parameters.OperationResult = CreateExistRecordResult(entity.Mail);
            }
        }
    }
}
