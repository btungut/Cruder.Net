using Cruder.Core.Contract;

namespace Cruder.Data.Repository
{
    public abstract class BaseCruderRepository<T> : EntityRepository<T> where
        T:class, IEntity
    {
        public BaseCruderRepository()
            : base(new CruderDbContext())
        {
        }

        public override void Dispose()
        {
            this.DbContext.Dispose();
            base.Dispose();
        }
    }
}
