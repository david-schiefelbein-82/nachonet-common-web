using System.Runtime.Serialization;

namespace Nachonet.Common.Web.Oidc.Errors
{
    [Serializable]
    internal class JwtValidationException : Exception
    {
        public JwtValidationException()
        {
        }

        public JwtValidationException(string? message) : base(message)
        {
        }

        public JwtValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}