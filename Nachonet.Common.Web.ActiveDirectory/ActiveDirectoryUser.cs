using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Nachonet.Common.Web.ActiveDirectory
{
    public class ActiveDirectoryUser(string userName, string[] groups)
    {
        private static readonly JsonSerializerOptions _serializationOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public string UserName { get; } = userName;

        public string? Email { get; set; }

        public string? PreferredName { get; set; }

        public string? GivenName { get; set; }

        public string? Surname { get; set; }

        public string[] Groups { get; } = groups;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, _serializationOptions);
        }
    }
}
