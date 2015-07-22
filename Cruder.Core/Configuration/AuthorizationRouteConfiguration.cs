using System.Configuration;

namespace Cruder.Core.Configuration
{
    public sealed class AuthorizationRouteConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("controller", IsRequired = true)]
        public string Controller
        {
            get
            {
                return (string)this["controller"];
            }
        }

        [ConfigurationProperty("loginAction", IsRequired = true)]
        public string LoginAction
        {
            get
            {
                return (string)this["loginAction"];
            }
        }

        [ConfigurationProperty("logoutAction", IsRequired = true)]
        public string LogoutAction
        {
            get
            {
                return (string)this["logoutAction"];
            }
        }
    }
}
