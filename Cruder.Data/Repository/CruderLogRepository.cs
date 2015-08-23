using System;
using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderLogRepository : BaseCruderRepository<Model.LogEntity, int>
    {
        public override Func<IQueryable<Model.LogEntity>, IOrderedQueryable<Model.LogEntity>> DefaultSorter
        {
            get
            {
                return q => q.OrderByDescending(x => x.UpdatedOn);
            }
        }
    }
}
