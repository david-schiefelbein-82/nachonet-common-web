using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Oidc
{
    /// <summary>
    /// Provides key storage and retrieval
    /// </summary>
    public interface IOidcKeyCache
    {
        bool HasExpired(string kid);

        void SetAll(IEnumerable<OidcKey> values);
        bool TryGet(string key, [NotNullWhen(true)] out OidcKey? value);
        void Save();
        bool TrySave();
    }
}