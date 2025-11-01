using System.Runtime.Serialization;

namespace Nachonet.Common.Web.Oidc.Errors
{
    [Serializable]
    internal class OAuthClientException : Exception
    {
        public OAuthClientException()
        {
        }

        public OAuthClientException(string? message) : base(message)
        {
        }

        public OAuthClientException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}