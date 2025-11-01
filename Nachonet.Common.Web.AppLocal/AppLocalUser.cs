using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Nachonet.Common.Web.AppLocal
{
    public class AppLocalUser(string userName, string[] groups) : IAppLocalUser
    {
        private static readonly JsonSerializerOptions _serialiserOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public string UserName { get; set; } = userName;

        public string? Email { get; set; }

        public string? PreferredName { get; set; }

        public string? GivenName { get; set; }

        public string? Surname { get; set; }

        public string[] Groups { get; set; } = groups;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, _serialiserOptions);
        }
    }
}
