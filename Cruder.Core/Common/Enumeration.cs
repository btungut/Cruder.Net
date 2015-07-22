using System.ComponentModel;

namespace Cruder.Core
{
    public enum ActionType
    {
        Create,
        Retrieve,
        Update,
        Delete,
        Unknown,
    }

    public enum LogType
    {
        Error = -1,

        Info = 0,

        Success = 1
    }

    public enum Priority
    {
        None = -2,

        Low = -1,

        Normal = 0,

        High = 1
    }

    public enum OrderType
    {
        Ascending,

        Descending
    }

    public enum LoggingLevel
    {
        NoLog,

        Normal,
    }

    public enum ProductEnvironment
    {
        Development,

        Production
    }

    public enum LogModule
    {
        Unknown,

        Repository,

        AdminUI,

        WebUI,

        Business,

        Framework,

        ExceptionHandler
    }

    public enum AuthorizationErrorCode
    {
        AccessLevel = 1
    }

    [DefaultValue(0)]
    public enum ControllerRegistrationSetting
    {
        None = 0,
        OnlyCruderControllers = 1,
        OnlyAssemblyControllers = 2,
        All = 3
    }

    [DefaultValue(0)]
    public enum RepositoryRegistrationSetting
    {
        None = 0,
        OnlyCruderRepositories = 1,
        OnlyAssemblyRepositories = 2,
        All = 3
    }
}
