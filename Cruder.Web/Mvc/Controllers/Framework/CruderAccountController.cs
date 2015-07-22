using Cruder.Core;
using Cruder.Resource;
using Cruder.Web.Auth;
using Cruder.Web.Auth.Attributes;
using Cruder.Web.ViewModel;
using System.Web.Mvc;

namespace Cruder.Web.Mvc.Controllers.Framework
{
    [AllowAnonymousAccess]
    public class CruderAccountController : BaseController
    {
        public ActionResult Login(
            string returnUrl,
            string encrypt,
            int? authorizationErrorCode,
            bool? initializeConfigKeys,
            bool? isDevelopmentEnvironment)
        {
            if (!string.IsNullOrEmpty(encrypt))
            {
                Response.Write(Definition.Cryptology.Encrypt(encrypt));
            }

            if (authorizationErrorCode.HasValue)
            {
                if (authorizationErrorCode.Value == (int)AuthorizationErrorCode.AccessLevel)
                {
                    base.WriteMessage(new MessageModel
                    {
                        Content = ResourceManager.GetString("Web.Mvc.Controller.CruderAccountController.AccessLevelMessage"),
                        Type = MessageType.Failure
                    });
                }
            }

            if (isDevelopmentEnvironment.HasValue)
            {
                Definition.IsDevelopmentEnvironment = isDevelopmentEnvironment.Value;
            }

            if (initializeConfigKeys.HasValue)
            {
                ConfigManager.Initialize();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(string returnUrl, LogonViewModel model)
        {
            ActionResult retVal = View();

            if (Session["Cruder.LoginAttempt"] != null && int.Parse(Session["Cruder.LoginAttempt"].ToString()) > 5)
            {
                base.WriteMessage(new MessageModel
                {
                    Content = ResourceManager.GetString("Web.Mvc.Controller.CruderAccountController.FiveTimesAttemptMessage"),
                    Type = MessageType.Failure
                });
            }
            else
            {
                bool login = BaseAuthorization.Login(model.Username, model.Password);

                if (login)
                {
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        retVal = Redirect(returnUrl);
                    }
                    else
                    {
                        retVal = RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    if (Session["Cruder.LoginAttempt"] != null)
                    {
                        Session["Cruder.LoginAttempt"] = int.Parse(Session["Cruder.LoginAttempt"].ToString()) + 1;
                    }
                    else
                    {
                        Session["Cruder.LoginAttempt"] = 1;
                    }

                    base.WriteMessage(new MessageModel
                    {
                        Content = ResourceManager.GetString("Web.Mvc.Controller.CruderAccountController.WrongCombination"),
                        Type = MessageType.Failure
                    });
                }
            }
            

            return retVal;
        }

        public ActionResult Logout()
        {
            BaseAuthorization.Logout();

            return RedirectToAction("Login");
        }
    }
}
