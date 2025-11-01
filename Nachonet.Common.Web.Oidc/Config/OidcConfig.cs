using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Oidc.Config
{
    public class OidcConfig : IOidcConfig
    {
        [JsonPropertyName("client-id")]
        public string ClientId { get; set; }

        [JsonPropertyName("client-secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("redirect-uri")]
        public string RedirectUri { get; set; }

        [JsonPropertyName("token-endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("jwks-uri")]
        public string JwksUri { get; set; }

        [JsonPropertyName("revocation-endpoint")]
        public string RevocationEndpoint { get; set; }

        [JsonPropertyName("authorization-endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("response-type")]
        public string ResponseType { get; set; }

        [JsonPropertyName("response-mode")]
        public string ResponseMode { get; set; }

        public OidcConfig()
        {
            ClientId = string.Empty;
            ClientSecret = string.Empty;
            RedirectUri = string.Empty;
            TokenEndpoint = string.Empty;
            Issuer = string.Empty;
            JwksUri = string.Empty;
            RevocationEndpoint = string.Empty;
            AuthorizationEndpoint = string.Empty;
            Scope = string.Empty;
            ResponseType = "code";
            ResponseMode = "";
        }
    }
}
