using Cruder.Core;
using Cruder.Core.Cryptology;
using Cruder.Web.Auth;
using System;
using System.Reflection;

namespace Cruder.Web.Model
{
    public sealed class InitializationSettings
    {
        public ControllerRegistrationSetting ControllerRegistration { get; set; }

        public RepositoryRegistrationSetting RepositoryRegistration { get; set; }

        public bool RegisterCustomContainerImplementations { get; set; }

        public bool RegisterEntityFrameworkContexts { get; set; }

        public BaseCryptology CryptologyProvider { get; set; }

        public BaseAuthorization AuthorizationFilter { get; set; }

        public Predicate<Assembly> AssemblyFilter { get; set; }
    }
}
