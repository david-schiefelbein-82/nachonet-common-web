namespace Nachonet.Common.Web.Oidc
{
    public interface IOidcClient
    {
        Task<OidcKey> FetchKeysAsync(string keyId, CancellationToken cancellationToken = default);
        string GetAuthenticationUrl(string state);
        Task<OidcCodeToken> GetTokenAsync(string code, CancellationToken cancellationToken = default);
        Task<string> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<string> RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task ValidateJwtAsync(OidcJwtToken token, CancellationToken cancellationToken = default);
    }
}