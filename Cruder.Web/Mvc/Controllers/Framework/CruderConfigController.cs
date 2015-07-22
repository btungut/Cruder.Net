using Cruder.Data.Repository;

namespace Cruder.Web.Mvc.Controllers.Framework
{
    public class CruderConfigController : CruderWebController<Cruder.Data.Model.ConfigEntity>
    {
        public CruderConfigController()
            : base(new CruderConfigRepository())
        {
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
        }
    }
}
