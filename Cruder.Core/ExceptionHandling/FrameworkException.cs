using System;

namespace Cruder.Core.ExceptionHandling
{
    public class FrameworkException : BaseException
    {
        public FrameworkException(string place, string message) : base(place, message) { }
        public FrameworkException(string place, string message, Exception innerException) : base(place, message, innerException) { }
    }
}
