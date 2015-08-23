using Cruder.Core.Contract;

namespace Cruder.Data.Repository
{
    public abstract class BaseCruderRepository<TEntity, TKey> : EntityRepository<TEntity, TKey> where
        TEntity : class, IEntity<TKey>
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
