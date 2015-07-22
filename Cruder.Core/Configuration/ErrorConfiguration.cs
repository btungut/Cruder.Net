using System.Configuration;

namespace Cruder.Core.Configuration
{
    public sealed class ErrorConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("notFoundPage", IsRequired = true)]
        public string NotFoundPage
        {
            get
            {
                return (string)this["notFoundPage"];
            }
        }

        [ConfigurationProperty("generalErrorPage", IsRequired = true)]
        public string GeneralErrorPage
        {
            get
            {
                return (string)this["generalErrorPage"];
            }
        }
    }
}
