namespace Nachonet.Common.Web.Oidc
{
    using Microsoft.Extensions.Logging;
    using Nachonet.Common.Web.Oidc.Config;
    using Nachonet.Common.Web.Oidc.Errors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Web;

    /// <summary>
    /// this class is capable of providing the callback URL for ASP to login with
    /// Once the IDP redirects the browser back to the site it can perform a lookup on the code (if ResponseType=code)
    /// and decode and validate the jwt token that was provided
    /// 
    /// This class does http lookup operations to:
    /// - convert the code into a token
    /// - get the token signature keys
    /// - revoke a token  (not tested)
    /// - refresh a token (not tested)
    /// </summary>
    public class OidcClient(ILogger<OidcClient> logger, IOidcConfig config, IOidcKeyCache keyCache) : IOidcClient
    {
        private readonly ILogger<OidcClient> _logger = logger;
        private readonly IOidcConfig _config = config;
        private readonly IOidcKeyCache _keyCache = keyCache;

        public async Task<string> RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string>
            {
                { "token", refreshToken },
                { "client_secret", _config.ClientSecret },
                { "client_id" , _config.ClientId }
            };

            var revokeClient = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = await revokeClient.PostAsync(_config.RevocationEndpoint, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync(cancellationToken);
                return responseString;
            }

            throw new OAuthClientException("Could not revoke the refresh token");
        }

        public async Task<string> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_secret", _config.ClientSecret },
                { "client_id" , _config.ClientId }
            };

            var refreshClient = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = await refreshClient.PostAsync(_config.TokenEndpoint, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            throw new OAuthClientException("Could not refresh the tokens");
        }

        /// <summary>
        /// Gets the configured URL to redirect to authenticate with
        /// </summary>
        public string GetAuthenticationUrl(string state)
        {
            var nonce = new Random().Next(1, 10000);
            var queryString = new Dictionary<string, string?>()
            {
                { "client_id", _config.ClientId },
                { "response_type", _config.ResponseType },
                { "scope", _config.Scope },
                { "redirect_uri", _config.RedirectUri },
                { "nonce", nonce.ToString() },
                { "state", state },
            };
            string[] dontEncode = ["scope", "response_type"];

            if (string.Equals(_config.ResponseMode, "form_post", StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(_config.ResponseMode, "post", StringComparison.CurrentCultureIgnoreCase))
                queryString["response_mode"] = "form_post";

            var items = queryString
                .Where(x => x.Value != null)
                .Select(x =>
            {
                if (x.Value == null)
                    return Uri.EscapeDataString(x.Key) + "=" + x.Value;
                else if (dontEncode.Any(y => y.Equals(x.Key)))
                    return Uri.EscapeDataString(x.Key) + "=" + x.Value;
                else
                    return Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value);
            });
            string url = _config.AuthorizationEndpoint + "?" + string.Join("&", items);
            return url;
        }

        /// <summary>
        /// Lookup the jwt token from the code returned in authentication callback
        /// </summary>
        public async Task<OidcCodeToken> GetTokenAsync(string code, CancellationToken cancellationToken = default)
        {
            try
            {
                var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _config.ClientId},
                { "client_secret", _config.ClientSecret },
                { "code" , code },
                { "redirect_uri", _config.RedirectUri }
            };


                var tokenClient = new HttpClient();
                var content = new FormUrlEncodedContent(values);
                var response = await tokenClient.PostAsync(_config.TokenEndpoint, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync(cancellationToken);
                    return OidcCodeToken.Parse(result);
                }

                string errorMsg = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError("GetToken: Token request failed {StatusCode} {ErrorMesasge}", response.StatusCode, errorMsg);
                throw new OAuthClientException(string.Format("Token request failed {0:d} {0} {1}", response.StatusCode, errorMsg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetToken");
                throw new OAuthClientException(string.Format("Token request failed {0} {1}", ex.GetType().Name, ex.Message));
            }
        }

        /// <summary>
        /// Retrieve keys from the web to perform JWT validation
        /// </summary>
        public async Task<OidcKey> FetchKeysAsync(string keyId, CancellationToken cancellationToken = default)
        {
            var jwksclient = new HttpClient();
            jwksclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = jwksclient.GetAsync(_config.JwksUri, cancellationToken).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = await responseContent.ReadAsStringAsync(cancellationToken);

                var jResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
                if (jResponse.ValueKind != JsonValueKind.Object)
                {
                    throw new JwtValidationException("Could not contact JWKS endpoint");
                }

                if (!jResponse.TryGetProperty("keys", out JsonElement keys))
                {
                    throw new JwtValidationException("Could not contact JWKS endpoint");
                }

                if (keys.ValueKind == JsonValueKind.Array)
                {
                    var kvps = keys.EnumerateArray()
                        .Where(x =>
                        {
                            var jkid = x.GetProperty("kid");
                            return jkid.ValueKind == JsonValueKind.String;
                        })
                        .Select(x =>
                            new OidcKey(x.GetProperty("kid").ToString(), DateTime.Now, x));
                    _keyCache.SetAll(kvps);
                    _keyCache.TrySave();
                }

                if (_keyCache.TryGet(keyId, out var actualValue))
                    return actualValue;
                else
                    throw new JwtValidationException("Key not found in JWKS endpoint");
            }

            throw new JwtValidationException("Could not contact JWKS endpoint");
        }

        /// <summary>
        /// Decode a base-64 string into UTF-8
        /// </summary>
        public static string SafeDecodeBase64(string str)
        {
            return Encoding.UTF8.GetString(GetPaddedBase64String(str));
        }

        /// <summary>
        /// Pad a base64 string out for base64 compatability
        /// </summary>
        private static byte[] GetPaddedBase64String(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "===="[(base64Url.Length % 4)..];
            string base64 = padded.Replace("_", "/").Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        public async Task ValidateJwtAsync(OidcJwtToken token, CancellationToken cancellationToken = default)
        {
            var jKid = token.Header.GetProperty("kid");
            string keyId = jKid.ValueKind == JsonValueKind.String ? jKid.ToString() :
                throw new JwtValidationException("Key not found in TOKEN");

            if (_keyCache.HasExpired(keyId) || !_keyCache.TryGet(keyId, out OidcKey? key))
            {
                key = await FetchKeysAsync(keyId, cancellationToken);
            }

            token.Payload.TryGetProperty("iss", out JsonElement jIss);
            if (jIss.ValueKind != JsonValueKind.String)
            {
                throw new JwtValidationException("Issuer not validated");
            }

            string iss = jIss.ToString();
            if (!string.Equals(iss, _config.Issuer))
            {
                throw new JwtValidationException("Issuer \"" + iss + "\" does not match expected \"" + _config.Issuer + "\"");
            }

            var jmodulus = key.Properties.GetProperty("n");
            var jexponent = key.Properties.GetProperty("e");
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(
              new RSAParameters()
              {
                  Modulus = GetPaddedBase64String(jmodulus.ToString()),
                  Exponent = GetPaddedBase64String(jexponent.ToString())
              });

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token.HeaderBase64 + '.' + token.PayloadBase64));

            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");
            if (rsaDeformatter.VerifySignature(hash, GetPaddedBase64String(token.Signature)))
            {
                // Jwt Validated
                return;
            }
            else
            {
                throw new JwtValidationException("Could not validate signature of JWT");
            }
        }
    }
}