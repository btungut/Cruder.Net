using System;
using System.Linq;

namespace Cruder.Data.Repository
{
    public class CruderLogRepository : BaseCruderRepository<Cruder.Data.Model.LogEntity>
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
