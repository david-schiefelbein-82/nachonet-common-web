using Microsoft.Extensions.Logging.Abstractions;
using Nachonet.Common.Web.ActiveDirectory;
using Nachonet.Common.Web.AppLocal.Config;
using Nachonet.Common.Web.Configuration;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Nachonet.Common.Web.Test
{
    [Flags]
    public enum WebServerLoginType
    {
        OIDC = 1,
        ActiveDirectory = 2,
        Both = 4,
        LocaUser = 8,
    }

    public class TestObj
    {
        public WebServerLoginType LoginType { get; set; }
    }

    [TestClass]
    public class AppLocalConfigTest
    {
        private static readonly JsonSerializerOptions _serializationOptions = new ()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter() },
        };

        [TestMethod]
        public void TestFlagsSerialiser()
        {

            var obj = new TestObj() { LoginType = WebServerLoginType.OIDC | WebServerLoginType.ActiveDirectory };
            var json = System.Text.Json.JsonSerializer.Serialize<TestObj>(obj, _serializationOptions);

            Assert.IsNotNull(json);
        }

        [TestMethod]
        public void TestSerialiser()
        {
            var config = new AppLocalConfig()
            {
                Users =
                [
                    new AppLocalUserConfig()
                    {
                        Username = "test",
                        GivenName = "Joe",
                        Surname = "Bloggs",
                        Groups = ["group-a", "group-b"],
                        Password = "password",
                        PasswordSalt = string.Empty,
                        PreferredName = "joe-bloggs",
                        EmailAddress = "joe.blogs@test.net",
                    }
                ]
            };

            var json = System.Text.Json.JsonSerializer.Serialize<AppLocalConfig>(config);
            var expected = "{\"users\":[{\"username\":\"test\",\"password\":\"password\",\"password-salt\":\"\",\"email-address\":\"joe.blogs@test.net\",\"given-name\":\"Joe\",\"surname\":\"Bloggs\",\"preferred-name\":\"joe-bloggs\",\"groups\":[\"group-a\",\"group-b\"]}]}";
            Assert.AreEqual(expected, json);
        }


        [TestMethod]
        public void TestDeserialiser()
        {
            string[] expectedGroups = ["group-a", "group-b"];
            var json = "{\"users\":[{\"username\":\"test\",\"password\":\"password\",\"password-salt\":\"\",\"email-address\":\"joe.blogs@test.net\",\"given-name\":\"Joe\",\"surname\":\"Bloggs\",\"preferred-name\":\"joe-bloggs\",\"groups\":[\"group-a\",\"group-b\"]}]}";
            var config = System.Text.Json.JsonSerializer.Deserialize<AppLocalConfig>(json);
            Assert.IsNotNull(config);
            if (config == null)
                throw new Exception();

            Assert.AreEqual(1, config.Users.Count);
            Assert.AreEqual("test", config.Users[0].Username);
            Assert.AreEqual("Joe", config.Users[0].GivenName);
            Assert.AreEqual("Bloggs", config.Users[0].Surname);
            Assert.AreEqual("Bloggs", config.Users[0].Surname);
            CollectionAssert.AreEqual(expectedGroups, config.Users[0].Groups);
            Assert.AreEqual("password", config.Users[0].Password);
            Assert.AreEqual("", config.Users[0].PasswordSalt);
            Assert.AreEqual("joe-bloggs", config.Users[0].PreferredName);
            Assert.AreEqual("joe.blogs@test.net", config.Users[0].EmailAddress);
        }
    }
}