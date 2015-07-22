using Cruder.Core;
using System.Web.Mvc;

namespace Cruder.Web.Model
{
    public sealed class ActionParameters
    {
        public Result<int> OperationResult { get; set; }
        public ActionResult ContinuationActionResult { get; set; }
    }
}
