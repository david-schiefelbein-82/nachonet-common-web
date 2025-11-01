using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nachonet.Common.Web.ActiveDirectory.Config
{
    public class ActiveDirectoryConfig : IActiveDirectoryConfig
    {
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        public ActiveDirectoryConfig()
        {
            Domain = string.Empty;
        }
    }
}
