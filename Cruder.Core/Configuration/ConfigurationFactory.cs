using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruder.Core.Configuration
{
    public static class ConfigurationFactory
    {
        public static ApplicationConfiguration Application
        {
            get
            {
                object section = GetSection("cruder/application");
                return section == null ? null : (ApplicationConfiguration)section;
            }
        }

        public static ErrorConfiguration Error
        {
            get
            {
                object section = GetSection("cruder/error");
                return section == null ? null : (ErrorConfiguration)section;
            }
        }

        public static LoggerConfiguration Logger
        {
            get
            {
                object section = GetSection("cruder/logger");
                return section == null ? null : (LoggerConfiguration)section;
            }
        }

        public static AuthorizationRouteConfiguration AuthorizationRoute
        {
            get
            {
                object section = GetSection("cruder/authorizationRoute");
                return section == null ? null : (AuthorizationRouteConfiguration)section;
            }
        }

        private static object GetSection(string sectionName)
        {
            return System.Configuration.ConfigurationManager.GetSection(sectionName);
        }
    }
}
