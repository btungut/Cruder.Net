using System;

namespace Cruder.Core.ExceptionHandling
{
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public abstract class BaseException : Exception
    {
        public string Place { get; set; }

        public string ExceptionType
        {
            get
            {
                return GetType().Name;
            }
        }

        public BaseException(string place, string message)
            : base(message)
        {
            this.Place = place;
        }

        public BaseException(string place, string message, Exception innerexception)
            : base(message, innerexception)
        {
            this.Place = place;
        }
    }
}
