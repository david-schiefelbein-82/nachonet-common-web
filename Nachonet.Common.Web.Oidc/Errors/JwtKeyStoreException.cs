using System.Runtime.Serialization;

namespace Nachonet.Common.Web.Oidc.Errors
{
    [Serializable]
    internal class JwtKeyStoreException : Exception
    {
        public JwtKeyStoreException()
        {
        }

        public JwtKeyStoreException(string? message) : base(message)
        {
        }

        public JwtKeyStoreException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}