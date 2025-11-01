using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Oidc
{
    public class OidcKey
    {
        public OidcKey()
        {
            Id = string.Empty;
            RefreshTime = new DateTime(1970, 1, 1);
            Properties = new JsonElement();
        }

        public OidcKey(string id, DateTime timestamp, JsonElement info)
        {
            Id = id;
            RefreshTime = timestamp;
            Properties = info;
        }

        [JsonPropertyName("kid")]
        public string Id { get; set; }

        [JsonPropertyName("refresh-time")]
        public DateTime RefreshTime { get; set; }

        [JsonPropertyName("properties")]
        public JsonElement Properties { get; set; }
    }
}