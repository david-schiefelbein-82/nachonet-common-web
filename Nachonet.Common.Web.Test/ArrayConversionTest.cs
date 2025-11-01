using Microsoft.Extensions.Logging.Abstractions;
using Nachonet.Common.Web.ActiveDirectory;
using Nachonet.Common.Web.AppLocal.Config;
using Nachonet.Common.Web.Configuration;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Nachonet.Common.Web.Test
{
    [TestClass]
    public class ArrayConversionTest
    {
        [TestMethod]
        public void TestInterfaceToClass()
        {
            var arr1 = new IAppLocalUserConfig[]
            {
                new AppLocalUserConfig() { Username = "joe" }
            };

            var arr2 = Array.ConvertAll(arr1, x => (AppLocalUserConfig)x);
            Assert.AreEqual(1, arr2.Length);
            Assert.AreEqual("joe", arr2[0].Username);
        }

        [TestMethod]
        public void TestClassToInterface()
        {
            var arr1 = new AppLocalUserConfig[]
            {
                new() { Username = "joe" }
            };

            var arr2 = (IAppLocalUserConfig[])arr1;
            Assert.AreEqual(1, arr2.Length);
            Assert.AreEqual("joe", arr2[0].Username);
        }
    }
}