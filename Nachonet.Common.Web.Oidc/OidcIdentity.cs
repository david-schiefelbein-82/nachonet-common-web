using System.Security.Principal;

namespace Nachonet.Common.Web.Oidc
{
    public class OidcIdentity(string name) : IIdentity
    {
        public string? AuthenticationType => "oidc";

        public bool IsAuthenticated => true;

        public string? Name { get; } = name;
    }
}
