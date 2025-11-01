using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Oidc
{
    /// <summary>
    /// Represents a jwt token with:
    /// - Header
    /// - Payload
    /// - Signature
    /// </summary>
    public class OidcJwtToken(JsonElement header, JsonElement payload, string headerBase64, string payloadBase64, string signature)
    {
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, SerializerOptions);
        }

        public JsonElement Header { get; set; } = header;

        public JsonElement Payload { get; set; } = payload;

        [JsonIgnore]
        public string HeaderBase64 { get; set; } = headerBase64;

        [JsonIgnore]
        public string PayloadBase64 { get; set; } = payloadBase64;

        [JsonIgnore]
        public string Signature { get; set; } = signature;

        /// <summary>
        /// Tries to parse a jwt token
        /// expected token format Header.Payload.Signature
        /// </summary>
        public static bool TryParse(string jwt, [NotNullWhen(true)] out OidcJwtToken? token)
        {
            try
            {
                string[] jwtParts = jwt.Split('.');
                if (jwtParts.Length != 3)
                {
                    // token format bad
                    token = null;
                    return false;
                }

                var decodedHeader = SafeDecodeBase64(jwtParts[0]);
                var decodedPayload = SafeDecodeBase64(jwtParts[1]);
                string signature = jwtParts[2];
                var header = JsonSerializer.Deserialize<JsonElement>(decodedHeader, SerializerOptions);
                var payload = JsonSerializer.Deserialize<JsonElement>(decodedPayload, SerializerOptions);
                token = new OidcJwtToken(header, payload, jwtParts[0], jwtParts[1], signature);
                return true;
            }
            catch (Exception)
            {
                token = null;
                return false;
            }
        }

        public static string SafeDecodeBase64(string str)
        {
            return Encoding.UTF8.GetString(GetPaddedBase64String(str));
        }

        private static byte[] GetPaddedBase64String(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "===="[(base64Url.Length % 4)..];
            string base64 = padded.Replace("_", "/").Replace("-", "+");
            return Convert.FromBase64String(base64);
        }
    }
}
