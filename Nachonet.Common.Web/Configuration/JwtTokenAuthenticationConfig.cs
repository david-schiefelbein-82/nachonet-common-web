
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Configuration
{
    public class JwtTokenAuthenticationConfig : IJwtTokenAuthenticationConfig
    {
        [JsonPropertyName("web-site-domain")]
        public string WebSiteDomain { get; set; }


        [JsonPropertyName("token-key")]
        public string TokenKey { get; set; }


        [JsonPropertyName("user-name-claim")]
        public string UserNameClaim { get; set; }

        public JwtTokenAuthenticationConfig()
        {
            TokenKey = string.Empty;
            WebSiteDomain = string.Empty;
            UserNameClaim = "preferred_username";
        }

        public static string GenerateKey(int lenBits)
        {
            var key = RandomNumberGenerator.GetBytes(lenBits / 8);
            return Convert.ToBase64String(key, Base64FormattingOptions.None);
        }

        public byte[] GetTokenSigningKey()
        {
            return Convert.FromBase64String(TokenKey);
        }
    }
}
