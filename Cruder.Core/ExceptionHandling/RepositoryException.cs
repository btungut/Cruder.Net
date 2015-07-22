using System;

namespace Cruder.Core.ExceptionHandling
{
    public class RepositoryException : BaseException
    {
        public RepositoryException(string place, string message) : base(place, message) { }
        public RepositoryException(string place, string message, Exception innerException) : base(place, message, innerException) { }
    }
}
