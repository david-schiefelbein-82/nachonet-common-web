namespace Nachonet.Common.Web.AppLocal
{
    public interface IAppLocalUser
    {
        string UserName { get; }

        string? Email { get; set; }

        string? PreferredName { get; }

        string? GivenName { get; }

        public string? Surname { get; }

        public string[] Groups { get; }
    }
}