using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Configuration
{
    public interface IJwtTokenAuthenticationConfig
    {

        public string WebSiteDomain { get; set; }

        public string TokenKey { get; set; }

        public string UserNameClaim { get; set; }

        public byte[] GetTokenSigningKey();
    }
}
