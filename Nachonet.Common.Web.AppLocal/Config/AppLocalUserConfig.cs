using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.AppLocal.Config
{
    public class AppLocalUserConfig : IAppLocalUserConfig
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("password-salt")]
        public string PasswordSalt { get; set; }

        [JsonPropertyName("email-address")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("given-name")]
        public string? GivenName { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("preferred-name")]
        public string? PreferredName { get; set; }

        [JsonPropertyName("groups")]
        public string[] Groups { get; set; }

        public AppLocalUserConfig()
        {
            Username = string.Empty;
            Password = string.Empty;
            PasswordSalt = string.Empty;
            EmailAddress = string.Empty;
            GivenName = string.Empty;
            Surname = string.Empty;
            PreferredName = string.Empty;
            Groups = [];
        }
    }
}