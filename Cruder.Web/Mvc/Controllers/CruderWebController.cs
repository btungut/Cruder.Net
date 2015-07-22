using Cruder.Core.Contract;
using Cruder.Core.Module;
using Cruder.Web.ViewModel;

namespace Cruder.Web.Mvc.Controllers
{
    public abstract class CruderWebController<T> : CruderWebController<T, ListViewModel<T>, DetailViewModel<T>> where T : IEntity
    {
        public CruderWebController()
            : this(IoC.Resolve<ICruderRepository<T>>())
        {
        }

        public CruderWebController(ICruderRepository<T> repository)
            : base(repository)
        {
        }

        protected override ListViewModel<T> GetListViewModelInstance()
        {
            return new ListViewModel<T>();
        }

        protected override DetailViewModel<T> GetDetailViewModelInstance()
        {
            return new DetailViewModel<T>();
        }
    }
}
