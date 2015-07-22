using Cruder.Core.Model;
using System.Web;

namespace Cruder.Web
{
    public static class UserSessionManager
    {
        private const string UserSessionKey = "Cruder.User";

        internal static void Bind(UserModel user)
        {
            HttpContext.Current.Session[UserSessionKey] = user;
        }

        internal static void Unbind()
        {
            HttpContext.Current.Session[UserSessionKey] = null;
        }

        internal static UserModel GetCurrent()
        {
            return HttpContext.Current.Session[UserSessionKey] == null ? null : HttpContext.Current.Session[UserSessionKey] as UserModel;
        }
    }
}
