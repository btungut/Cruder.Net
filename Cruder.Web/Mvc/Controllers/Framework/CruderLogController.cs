using Cruder.Data.Repository;
using System.Web.Mvc;

namespace Cruder.Web.Mvc.Controllers.Framework
{
    public class CruderLogController : CruderWebController<Cruder.Data.Model.LogEntity>
    {
        public CruderLogController()
            : base(new CruderLogRepository())
        {
        }

        public ActionResult ServerVariables()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
        }
    }
}
