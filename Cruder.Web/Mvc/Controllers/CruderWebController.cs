using Cruder.Core.Contract;
using Cruder.Core.Module;
using Cruder.Web.ViewModel;

namespace Cruder.Web.Mvc.Controllers
{
    public abstract class CruderWebController<TEntity> : CruderWebController<TEntity, ListViewModel<TEntity>, DetailViewModel<TEntity>> 
        where TEntity : IEntity
    {
        public CruderWebController()
            : this(IoC.Resolve<ICruderRepository<TEntity>>())
        {
        }

        public CruderWebController(ICruderRepository<TEntity> repository)
            : base(repository)
        {
        }

        protected override ListViewModel<TEntity> GetListViewModelInstance()
        {
            return new ListViewModel<TEntity>();
        }

        protected override DetailViewModel<TEntity> GetDetailViewModelInstance()
        {
            return new DetailViewModel<TEntity>();
        }
    }
}
