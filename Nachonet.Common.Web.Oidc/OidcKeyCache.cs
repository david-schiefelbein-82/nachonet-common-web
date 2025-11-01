using Nachonet.Common.Web.Oidc.Errors;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nachonet.Common.Web.Oidc
{
    public class OidcKeyCache(string configFileName) : IOidcKeyCache
    {
        private readonly Dictionary<string, OidcKey> _keys = [];

        private DateTime _lastRefreshTime = new (1970, 1, 1);

        private TimeSpan _expiryTime = TimeSpan.FromDays(1);

        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// the json file where cache is stored
        /// </summary>
        private readonly string _configFileName = configFileName;

        /// <summary>
        /// Get if the cache has expired
        /// </summary>
        public bool HasExpired(string kid)
        {
            if (TryGet(kid, out OidcKey? keyInfo))
            {
                return keyInfo.RefreshTime.Add(_expiryTime) < DateTime.Now;
            }

            return true;
        }

        /// <summary>
        /// save to json file, throws an exeption on failure
        /// </summary>
        public void Save()
        {
            var keyStore = new OidcKeyStore
            {
                ExpiryTime = _expiryTime,
                LastRefreshTime = _lastRefreshTime,
                Keys = (from x in _keys orderby x.Key select x.Value).ToArray()
            };

            using var stream = new FileStream(_configFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            JsonSerializer.Serialize(stream, keyStore, SerializerOptions);
        }

        /// <summary>
        /// tries to save the keys to a json file
        /// </summary>
        public bool TrySave()
        {
            try
            {
                var keyStore = new OidcKeyStore
                {
                    ExpiryTime = _expiryTime,
                    LastRefreshTime = _lastRefreshTime,
                    Keys = (from x in _keys orderby x.Key select x.Value).ToArray()
                };

                using var stream = new FileStream(_configFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                JsonSerializer.Serialize(stream, keyStore, SerializerOptions);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string DefaultConfigName
        {
            get => Path.Combine(Environment.CurrentDirectory, "config", "oidc-keys.json");
        }

        /// <summary>
        /// load the keys from a json file
        /// </summary>
        public static OidcKeyCache Load(string configFileName)
        {
            using var stream = new FileStream(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var keystore = JsonSerializer.Deserialize<OidcKeyStore>(stream, SerializerOptions);
            if (keystore == null || keystore.Keys == null)
                throw new JwtKeyStoreException("Unable to load keystore from config file");

            var cache = new OidcKeyCache(configFileName);
            cache.SetAll(keystore.Keys);
            cache._lastRefreshTime = keystore.LastRefreshTime;
            cache._expiryTime = keystore.ExpiryTime;
            return cache;
        }

        /// <summary>
        /// try to retrieve a key from the cache
        /// </summary>
        public bool TryGet(string key, [NotNullWhen(true)] out OidcKey? value)
        {
            lock (this)
            {
                return _keys.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// Set all keys to updated values
        /// </summary>
        public void SetAll(IEnumerable<OidcKey> values)
        {
            lock (this)
            {
                foreach (var keyInfo in values)
                {
                    _keys[keyInfo.Id] = keyInfo;
                }

                _lastRefreshTime = DateTime.Now;
            }
        }

        public class OidcKeyStore
        {
            [JsonPropertyName("keys")]
            public OidcKey[] Keys { get; set; }

            [JsonPropertyName("last-refresh-time")]
            public DateTime LastRefreshTime { get; set; }

            [JsonPropertyName("expiry-time")]
            public TimeSpan ExpiryTime { get; set; }

            public OidcKeyStore()
            {
                Keys = [];
            }
        }
    }
}
