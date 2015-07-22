using Cruder.Core;
using Cruder.Web.ViewModel;
using System.Web.Mvc;

namespace Cruder.Web.Mvc.Controllers
{
    public abstract class BaseController : System.Web.Mvc.Controller
    {
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            ViewBag.Controller = filterContext.RouteData.Values["controller"].ToString();
            ViewBag.Action = filterContext.RouteData.Values["action"].ToString();

            base.OnActionExecuting(filterContext);
        }

        public ActionResult RedirectToReferrer()
        {
            return RedirectToReferrer(RedirectToAction("Index"));
        }

        public ActionResult RedirectToReferrer(string ifRefferrerNotFoundToBeReturnedPage)
        {
            return RedirectToReferrer(Redirect(ifRefferrerNotFoundToBeReturnedPage));
        }

        public ActionResult RedirectToReferrer(ActionResult ifRefferrerNotFoundToBeReturnedPage)
        {
            ActionResult retVal = ifRefferrerNotFoundToBeReturnedPage;

            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri != null)
            {
                retVal = Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return retVal;
        }

        public void WriteMessage(Result result)
        {
            WriteMessage(new MessageModel(result));
        }

        public void WriteMessage(string content, MessageType type)
        {
            WriteMessage(new MessageModel(content, type));
        }

        public void WriteMessage(MessageModel model)
        {
            TempData["MessageModel"] = model;
        }
    }
}
