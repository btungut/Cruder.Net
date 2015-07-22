using Cruder.Core;
using Cruder.Core.Configuration;
using Cruder.Core.Cryptology;
using System.Web;

namespace Cruder.Web
{
    public static class Definition
    {
        public static BaseCryptology Cryptology
        {
            get
            {
                return (HttpContext.Current.ApplicationInstance as CruderHttpApplication).InitializationSettings.CryptologyProvider;
            }
        }

        public static bool IsDevelopmentEnvironment
        {
            get
            {
                bool retVal = false;

                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["Cruder.IsDevelopmentEnvironment"] != null)
                {
                    retVal = (bool)HttpContext.Current.Session["Cruder.IsDevelopmentEnvironment"];
                }
                else if (ConfigurationFactory.Application.ProductEnvironment == ProductEnvironment.Development)
                {
                    retVal = true;
                }

                return retVal;
            }
            set
            {
                HttpContext.Current.Session["Cruder.IsDevelopmentEnvironment"] = value;
            }
        }

        public static bool IsDebugEnvironment
        {
            get
            {
                bool retVal = false;
#if DEBUG
                retVal = true;
#endif
                return retVal;
            }
        }
    }
}
