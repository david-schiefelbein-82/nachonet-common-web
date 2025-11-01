using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Nachonet.Common.Web.ActiveDirectory;
using Nachonet.Common.Web.AppLocal;
using Nachonet.Common.Web.Oidc;

namespace Nachonet.Common.Web
{
    public interface IJwtSecurityTokenProvider
    {
        string CreateAuthenticationToken(OidcJwtToken idToken);

        string CreateAuthenticationToken(ActiveDirectoryUser user);

        string CreateAuthenticationToken(IAppLocalUser user);

        Task<TokenValidationResult> ReadAndValidateTokenAsync(string tokenString, CancellationToken cancellationToken = default);
    }
}
