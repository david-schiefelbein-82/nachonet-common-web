namespace Nachonet.Common.Web.Configuration
{
    public interface IAuthorizationConfig
    {
        List<AuthEntityConfig> UsersAndGroups { get; set; }

        void Save();
    }
}