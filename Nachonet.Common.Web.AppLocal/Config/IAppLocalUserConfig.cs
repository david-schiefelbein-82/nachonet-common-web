using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.AppLocal.Config
{
    public interface IAppLocalUserConfig
    {
        string Username { get; }

        string Password { get; }

        string PasswordSalt { get; }

        string? EmailAddress { get; }

        string? GivenName { get; }

        string? Surname { get; set; }

        string? PreferredName { get; set; }

        string[] Groups { get; set; }
    }
}