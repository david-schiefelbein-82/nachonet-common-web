namespace Nachonet.Common.Web.AppLocal.Errors
{
    public class BasicAuthException : Exception
    {
        public BasicAuthException(string message) : base(message)
        {

        }
        public BasicAuthException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
