using System;
using System.Text;

namespace Cruder.Core
{
    public class Result
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public bool HasError { get; set; }

        public Result()
        {
            this.HasError = false;
        }

        public Result(string message, bool hasError)
        {
            this.Message = message;
            this.HasError = hasError;
        }

        public Result(Exception exception)
        {
            this.Exception = exception;
            this.HasError = true;

            StringBuilder builder = new StringBuilder();
            Exception iteration = exception;

            while (iteration != null)
            {
                builder.AppendLine(string.Format("- {0}", iteration.Message));
                iteration = iteration.InnerException;
            }

            this.Message = builder.ToString();
        }

        public override bool Equals(object obj)
        {
            bool retVal = false;

            var result = obj as Result;

            if (result.Message == this.Message &&
                result.Exception.GetHashCode() == this.Exception.GetHashCode() &&
                result.HasError == this.HasError)
            {
                retVal = true;
            }

            return retVal;
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        public Result() :base()
        {

        }

        public Result(T data) : base()
        {
            this.Data = data;
        }

        public Result(T data, string message, bool hasError) : base(message,hasError)
        {
            this.Data = data;
        }

        public Result(T data, Exception exception) : base(exception)
        {
            this.Data = data;
        }

        public override bool Equals(object obj)
        {
            bool retVal = false;

            var result = obj as Result<T>;

            if (base.Equals(result) &&
                result.Data.GetHashCode() == this.Data.GetHashCode())
            {
                retVal = true;
            }

            return retVal;
        }
    }
}
