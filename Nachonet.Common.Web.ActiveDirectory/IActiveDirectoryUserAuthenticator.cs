namespace Nachonet.Common.Web.ActiveDirectory
{
    public interface IActiveDirectoryUserAuthenticator
    {
        ActiveDirectoryUser Login(string username, string password);
    }
}
