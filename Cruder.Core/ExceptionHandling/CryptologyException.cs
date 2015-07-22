using System;

namespace Cruder.Core.ExceptionHandling
{
    public sealed class CryptologyException : BaseException
    {
        public CryptologyException(string place, string message) : base(place, message) { }
        public CryptologyException(string place, string message, Exception innerException) : base(place, message, innerException) { }
    }
}
