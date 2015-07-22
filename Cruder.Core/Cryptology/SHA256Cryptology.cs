using Cruder.Core.ExceptionHandling;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Cruder.Core.Cryptology
{
    public sealed class SHA256Cryptology : BaseCryptology
    {
        public SHA256Cryptology(string hashFormat)
            : base(hashFormat)
        {
        }

        public override string Encrypt(string data)
        {
            SHA256Managed sha256hasher = new SHA256Managed();

            try
            {
                UTF8Encoding encoder = new UTF8Encoding();

                byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(string.Format(base.hashFormat, data)));

                return Convert.ToBase64String(hashedDataBytes);
            }
            catch (Exception e)
            {
                var exception = new CryptologyException("SHA256Encryptor.Encrypt()", "An error occurred while encrypting.", e);
                exception.Data.Add("hashFormat", hashFormat);
                exception.Data.Add("data", data);

                throw exception;
            }
            finally
            {
                sha256hasher.Dispose();
            }
        }
    }
}
