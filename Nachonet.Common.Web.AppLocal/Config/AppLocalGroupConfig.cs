using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.AppLocal.Config
{
    public class AppLocalGroupConfig : IAppLocalGroupConfig
    {
        [JsonPropertyName("group-name")]
        public string GroupName { get; set; }

        [JsonPropertyName("preferred-name")]
        public string? PreferredName { get; set; }

        public AppLocalGroupConfig()
        {
            GroupName = string.Empty;
            PreferredName = null;
        }
    }
}