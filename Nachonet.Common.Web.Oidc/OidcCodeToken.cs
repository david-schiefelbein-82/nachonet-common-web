using Nachonet.Common.Web.Oidc.Errors;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Oidc
{
    public class OidcCodeToken
    {
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public OidcCodeToken()
        {
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, SerializerOptions);
        }

        public static OidcCodeToken Parse(string text)
        {
            return JsonSerializer.Deserialize<OidcCodeToken>(text) ?? 
                throw new JwtValidationException("OidcCodeToken invalid format");
        }

        [JsonPropertyName("access_token")]
        public string? AccessTokenBase64 { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshTokenBase64 { get; set; }
        
        [JsonPropertyName("id_token")]
        public string? IdTokenBase64 { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }
}
