using System;

namespace Cruder.Core.Cryptology
{
    public abstract class BaseCryptology
    {
        protected readonly string hashFormat;

        public BaseCryptology(string hashFormat)
        {
            if (!hashFormat.Contains("{0}"))
                throw new ArgumentException("'hashFormat' variable in 'BaseCryptology' must contain a formatter like that '{0}'");
            else
                this.hashFormat = hashFormat;
        }

        public abstract string Encrypt(string data);

    }
}
