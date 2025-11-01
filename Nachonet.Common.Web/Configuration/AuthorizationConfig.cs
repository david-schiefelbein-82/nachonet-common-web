using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Configuration
{
    public class AuthorizationConfig : IAuthorizationConfig
    {
        public static JsonSerializerOptions SerializationOptions => new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public AuthorizationConfig()
        {
            UsersAndGroups = [];
        }

        [JsonPropertyName("users-and-groups")]
        public List<AuthEntityConfig> UsersAndGroups { get; set; }

        public static AuthorizationConfig Load()
        {
            var authConfigPath = Path.Combine("Config", "authorization.json");
            return Load(authConfigPath);
        }

        public static AuthorizationConfig Load(string authConfigPath)
        {
            using var fileStream = new FileStream(authConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var config = JsonSerializer.Deserialize<AuthorizationConfig>(fileStream, SerializationOptions) ??
                throw new AuthorizationConfigException("unable to load config file");

            return config;
        }

        public void Save()
        {
            var authConfigPath = Path.Combine("Config", "authorization.json");
            Save(authConfigPath);
        }

        public void Save(string authConfigPath)
        {
            using var fileStream = new FileStream(authConfigPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            JsonSerializer.Serialize(fileStream, this, SerializationOptions);
        }
    }
}