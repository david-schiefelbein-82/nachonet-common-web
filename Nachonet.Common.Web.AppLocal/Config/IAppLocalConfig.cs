namespace Nachonet.Common.Web.AppLocal.Config
{
    public interface IAppLocalConfig
    {
        public List<AppLocalUserConfig> Users { get; }

        public List<AppLocalGroupConfig> Groups { get; }
    }
}