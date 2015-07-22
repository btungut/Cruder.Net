using System;

namespace Cruder.Core.Contract
{
    public interface ILogger
    {
        Result<int> Log(LogType logType, Priority priority, string description, string detail, string request, Enum module);
    }
}
