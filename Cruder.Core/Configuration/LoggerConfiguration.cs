using System;
using System.Configuration;

namespace Cruder.Core.Configuration
{
    public sealed class LoggerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("loggingLevel", IsRequired = true)]
        public LoggingLevel LoggingLevel
        {
            get
            {
                return (LoggingLevel)Enum.Parse(typeof(LoggingLevel), this["loggingLevel"].ToString());
            }
        }

        [ConfigurationProperty("tableName", IsRequired = true)]
        public string TableName
        {
            get
            {
                return (string)this["tableName"];
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

        [ConfigurationProperty("filePath", IsRequired = true)]
        public string FilePath
        {
            get
            {
                return (string)this["filePath"];
            }
        }
    }
}
