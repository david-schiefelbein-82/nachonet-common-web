namespace Nachonet.Common.Web.Oidc.Config
{
    public interface IOidcConfig
    {
        string ClientId { get; }
        string ClientSecret { get; }
        string RedirectUri { get; }
        string TokenEndpoint { get; }
        string Issuer { get; }
        string JwksUri { get; }
        string RevocationEndpoint { get; }
        string AuthorizationEndpoint { get; }
        string Scope { get; }
        string ResponseType { get; }
        string ResponseMode { get; }
    }
}