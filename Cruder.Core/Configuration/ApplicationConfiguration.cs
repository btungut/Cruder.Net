using System;
using System.Configuration;

namespace Cruder.Core.Configuration
{
    public sealed class ApplicationConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("productEnvironment", IsRequired = true)]
        public ProductEnvironment ProductEnvironment
        {
            get
            {
                return (ProductEnvironment)Enum.Parse(typeof(ProductEnvironment), this["productEnvironment"].ToString());
            }
        }

        [ConfigurationProperty("homePage", IsRequired = true)]
        public string HomePage
        {
            get
            {
                return (string)this["homePage"];
            }
        }

        [ConfigurationProperty("developmentEnvironmentUserId", IsRequired = true)]
        public int DevelopmentEnvironmentUserId
        {
            get
            {
                return (int)this["developmentEnvironmentUserId"];
            }
        }

        [ConfigurationProperty("connectionStringKey", IsRequired = true)]
        public string ConnectionStringKey
        {
            get
            {
                return (string)this["connectionStringKey"];
            }
        }
    }
}
