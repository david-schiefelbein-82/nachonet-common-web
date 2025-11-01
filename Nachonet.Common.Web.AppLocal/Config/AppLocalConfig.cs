using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.AppLocal.Config
{
    public class AppLocalConfig : IAppLocalConfig
    {
        [JsonPropertyName("users")]
        public List<AppLocalUserConfig> Users { get; set; }

        [JsonPropertyName("groups")]
        public List<AppLocalGroupConfig> Groups { get; set; }

        public AppLocalConfig()
        {
            Users = [];
            Groups = [];
        }
    }

    public class AppLocalUserConfigConverter : JsonConverter<List<IAppLocalUserConfig>>
    {
        [return: MaybeNull]
        public override List<IAppLocalUserConfig> Read(
          ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var l = JsonSerializer.Deserialize<List<AppLocalUserConfig>>(ref reader, options) ??
                throw new Exception("cannot read users - null");
            return (from x in l select (IAppLocalUserConfig)x).ToList();
        }

        public override void Write(
          Utf8JsonWriter writer, List<IAppLocalUserConfig> value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, options);
    }

    public class AppLocalGroupConfigConverter : JsonConverter<List<IAppLocalGroupConfig>>
    {
        [return: MaybeNull]
        public override List<IAppLocalGroupConfig> Read(
          ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var l = JsonSerializer.Deserialize<List<AppLocalGroupConfig>>(ref reader, options) ??
                throw new Exception("cannot read users - null");
            return (from x in l select (IAppLocalGroupConfig)x).ToList();
        }

        public override void Write(
          Utf8JsonWriter writer, List<IAppLocalGroupConfig> value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, options);
    }
}
