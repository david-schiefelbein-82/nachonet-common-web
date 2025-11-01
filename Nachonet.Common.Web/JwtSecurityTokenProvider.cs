using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Nachonet.Common.Web.ActiveDirectory;
using Nachonet.Common.Web.AppLocal;
using Nachonet.Common.Web.Configuration;
using Nachonet.Common.Web.Oidc;
using System.Security.Claims;
using System.Text.Json;

namespace Nachonet.Common.Web
{
    public class JwtSecurityTokenProvider(IJwtTokenAuthenticationConfig webConfig, IAuthorizationConfig authConfig) : IJwtSecurityTokenProvider
    {
        private readonly IJwtTokenAuthenticationConfig _webConfig = webConfig;
        private readonly IAuthorizationConfig _authConfig = authConfig;
        public const string KEY_GIVEN_NAME = "given_name";
        public const string KEY_FAMILY_NAME = "family_name";
        public const string KEY_GROUP = "group";
        public const string KEY_PREFERRED_USERNAME = "preferred_username";

        public string CreateAuthenticationToken(OidcJwtToken idToken)
        {
            var securityKey = new SymmetricSecurityKey(_webConfig.GetTokenSigningKey());
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _webConfig.WebSiteDomain,
                Subject = new ClaimsIdentity(GetUserClaims(idToken)),
                Audience = _webConfig.WebSiteDomain,
                IssuedAt = null,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler()
            {
                SetDefaultTimesOnTokenCreation = false
            };
            var tokenString = handler.CreateToken(descriptor);
            return tokenString;
        }

        public string CreateAuthenticationToken(ActiveDirectoryUser user)
        {
            var securityKey = new SymmetricSecurityKey(_webConfig.GetTokenSigningKey());
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _webConfig.WebSiteDomain,
                Subject = new ClaimsIdentity(GetUserClaims(user)),
                Audience = _webConfig.WebSiteDomain,
                IssuedAt = null,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler()
            {
                SetDefaultTimesOnTokenCreation = false
            };
            var tokenString = handler.CreateToken(descriptor);
            return tokenString;
        }

        public string CreateAuthenticationToken(IAppLocalUser user)
        {
            var securityKey = new SymmetricSecurityKey(_webConfig.GetTokenSigningKey());
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _webConfig.WebSiteDomain,
                Subject = new ClaimsIdentity(GetUserClaims(user)),
                Audience = _webConfig.WebSiteDomain,
                IssuedAt = null,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JsonWebTokenHandler()
            {
                SetDefaultTimesOnTokenCreation = false
            };
            var tokenString = handler.CreateToken(descriptor);
            return tokenString;
        }

        public async Task<TokenValidationResult> ReadAndValidateTokenAsync(string tokenString, CancellationToken cancellationToken = default)
        {
            var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
            var token = handler.ReadToken(tokenString) ?? throw new Exception("Unable to decode token");
            var validationParameters = GetValidationParameters();

            var tokenValidationResult = await handler.ValidateTokenAsync(token, validationParameters);

            if (!tokenValidationResult.IsValid)
                throw tokenValidationResult.Exception;

            return tokenValidationResult;

            throw new Exception("Unable to decode token");
        }


        private TokenValidationParameters GetValidationParameters()
        {
            var key = _webConfig.GetTokenSigningKey();
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _webConfig.WebSiteDomain,
                ValidateAudience = true,
                ValidAudience = _webConfig.WebSiteDomain,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        private List<Claim> GetUserClaims(ActiveDirectoryUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
            };

            if (user.Email != null)
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            if (user.GivenName != null)
                claims.Add(new Claim(ClaimTypes.GivenName, user.GivenName));

            if (user.Surname != null)
                claims.Add(new Claim(ClaimTypes.Surname, user.Surname));

            if (user.PreferredName != null)
                claims.Add(new Claim(KEY_PREFERRED_USERNAME, user.PreferredName));

            // if the username is matched in config, assign them roles
            AuthEntityConfig? userConfig;
            var roles = new List<string>();
            if ((userConfig = FindUserOrGroup(user.UserName, AuthEntityType.ActiveDirectoryUser)) != null)
            {
                foreach (var role in userConfig.Roles)
                {
                    roles.Add(role);
                }
            }

            // if any of the users groups is matched in config, assign them roles
            foreach (var groupId in user.Groups)
            {
                claims.Add(new Claim(KEY_GROUP, groupId));
                AuthEntityConfig? groupConfig;
                if ((groupConfig = FindUserOrGroup(groupId, AuthEntityType.ActiveDirectoryGroup)) != null)
                {
                    foreach (var role in groupConfig.Roles)
                    {
                        roles.Add(role);
                    }
                }
            }

            // add all unique roles (ignore duplicates)
            roles = roles.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private List<Claim> GetUserClaims(IAppLocalUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
            };

            if (user.Email != null)
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            if (user.GivenName != null)
                claims.Add(new Claim(ClaimTypes.GivenName, user.GivenName));

            if (user.Surname != null)
                claims.Add(new Claim(ClaimTypes.Surname, user.Surname));

            if (user.PreferredName != null)
                claims.Add(new Claim(KEY_PREFERRED_USERNAME, user.PreferredName));

            // if the username is matched in config, assign them roles
            AuthEntityConfig? userConfig;
            var roles = new List<string>();
            if ((userConfig = FindUserOrGroup(user.UserName, AuthEntityType.AppLocalUser)) != null)
            {
                foreach (var role in userConfig.Roles)
                {
                    roles.Add(role);
                }
            }

            // if any of the users groups is matched in config, assign them roles
            foreach (var groupId in user.Groups)
            {
                claims.Add(new Claim(KEY_GROUP, groupId));
                AuthEntityConfig? groupConfig;
                if ((groupConfig = FindUserOrGroup(groupId, AuthEntityType.AppLocalGroup)) != null)
                {
                    foreach (var role in groupConfig.Roles)
                    {
                        roles.Add(role);
                    }
                }
            }

            // add all unique roles (ignore duplicates)
            roles = roles.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private AuthEntityConfig? FindUserOrGroup(string userName, AuthEntityType type)
        {
            return _authConfig.UsersAndGroups.FirstOrDefault(x =>
                string.Equals(x.Id, userName, StringComparison.CurrentCultureIgnoreCase) && x.Type == type);
        }

        private List<Claim> GetUserClaims(OidcJwtToken idToken)
        {
            var claims = new List<Claim>();

            var userNameClaim = string.IsNullOrWhiteSpace(_webConfig.UserNameClaim) ? null : idToken.Payload.GetString(_webConfig.UserNameClaim);
            var email = idToken.Payload.GetString("email");
            var preferredUserName = idToken.Payload.GetString(KEY_PREFERRED_USERNAME);
            var name = idToken.Payload.GetString("name");

            string userName;
            if (userNameClaim != null)
                userName = userNameClaim;
            else if (preferredUserName != null)
                userName = preferredUserName;
            else if (name != null)
                userName = name;
            else if (email != null)
                userName = email;
            else
                userName = "anon";

            claims.Add(new Claim(ClaimTypes.Name, userName));
            var roles = new List<string>();
            AuthEntityConfig? userConfig;

            // if the username is matched in config, assign them roles
            if ((userConfig = FindUserOrGroup(userName, AuthEntityType.OidcUser)) != null)
            {
                foreach (var role in userConfig.Roles)
                {
                    roles.Add(role);
                }
            }

            // if the any of the groups is matched in config, assign them roles
            if (idToken.Payload.TryGetProperty("groups", out JsonElement jGroups))
            {
                if (jGroups.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in jGroups.EnumerateArray())
                    {
                        var groupId = item.ToString();
                        claims.Add(new Claim(KEY_GROUP, groupId));

                        AuthEntityConfig? groupConfig;
                        if ((groupConfig = FindUserOrGroup(groupId, AuthEntityType.OidcGroup)) != null)
                        {
                            foreach (var role in groupConfig.Roles)
                            {
                                roles.Add(role);
                            }
                        }
                    }
                }
            }

            // add all unique roles (ignore duplicates)
            roles = roles.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var prop in idToken.Payload.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    claims.Add(new Claim(prop.Name, prop.Value.ToString()));
                }
                else if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                            claims.Add(new Claim(prop.Name, item.ToString()));
                    }
                }
            }

            return claims;
        }
    }
}
