using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Configuration
{
    public class AuthEntityConfig
    {
        public AuthEntityConfig()
        {
            Roles = [];
            Id = string.Empty;
            Label = string.Empty;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("type")]
        public AuthEntityType Type { get; set; }

        [JsonPropertyName("roles")]
        public string[] Roles { get; set; }
    }
}