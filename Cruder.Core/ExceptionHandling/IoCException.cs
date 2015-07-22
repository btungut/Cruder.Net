using System;

namespace Cruder.Core.ExceptionHandling
{
    public class IoCException : BaseException
    {
        public IoCException(string place, string message) : base(place, message) { }
        public IoCException(string place, string message, Exception innerException) : base(place, message, innerException) { }
    }
}
