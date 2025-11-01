namespace Nachonet.Common.Web.ActiveDirectory.Errors
{
    public class ActiveDirectoryException : Exception
    {
        public ActiveDirectoryException(string message) : base(message)
        {

        }
        public ActiveDirectoryException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
