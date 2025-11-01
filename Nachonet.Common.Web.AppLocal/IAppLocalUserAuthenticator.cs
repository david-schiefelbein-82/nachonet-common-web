namespace Nachonet.Common.Web.AppLocal
{
    public interface IAppLocalUserAuthenticator
    {
        IAppLocalUser Login(string username, string password);
    }
}
