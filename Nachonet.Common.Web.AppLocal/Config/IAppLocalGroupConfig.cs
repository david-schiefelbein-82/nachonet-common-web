using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.AppLocal.Config
{
    public interface IAppLocalGroupConfig
    {
        string GroupName { get; }

        public string? PreferredName { get; }
    }
}