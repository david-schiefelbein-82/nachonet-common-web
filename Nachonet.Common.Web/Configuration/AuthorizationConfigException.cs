using System.Net;

namespace Nachonet.Common.Web.Configuration
{
    public class AuthorizationConfigException : Exception
    {
        public AuthorizationConfigException(string? message) : base(message)
        {
        }

        public AuthorizationConfigException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
