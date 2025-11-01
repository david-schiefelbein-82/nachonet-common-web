using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Nachonet.Common.Web.ActiveDirectory;
using Nachonet.Common.Web.Configuration;

namespace Nachonet.Common.Web.Test
{
    [TestClass]
    public class JwtSecurityTokenProviderTest
    {
        private static JwtSecurityTokenProvider CreateProvider(string? key)
        {
            IJwtTokenAuthenticationConfig webConfig = new JwtTokenAuthenticationConfig()
            {
                WebSiteDomain = "https://test.local",
                UserNameClaim = "https://test.local",
                TokenKey = key ?? JwtTokenAuthenticationConfig.GenerateKey(256)
            };
            IAuthorizationConfig authConfig = new AuthorizationConfig();
            authConfig.UsersAndGroups.Add(new AuthEntityConfig() { Id = "group1", Label = "group1", Roles = ["role1", "role2"], Type = AuthEntityType.ActiveDirectoryGroup });

            var provider = new JwtSecurityTokenProvider(webConfig, authConfig);
            return provider;
        }


        [TestMethod]
        public async Task TestGenerateJwtAsync()
        {
            var provider = CreateProvider(null);
            var user = new ActiveDirectoryUser("user", ["group1", "group2"]);
            var jwt = provider.CreateAuthenticationToken(user);

            var validationResult = await provider.ReadAndValidateTokenAsync(jwt);
            var roles = validationResult.Claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] as IList<object>;

            Assert.IsNotNull(roles);
            Assert.IsTrue(roles.Any(x => string.Equals(x, "role1")));
            Assert.IsTrue(roles.Any(x => string.Equals(x, "role2")));
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException), "Invalid currency.")]
        public async Task DecodeWithWrongKeyAsync()
        {

            // this jwt was built with what will surely be a different key
            string jwt = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL3Rlc3QubG9jYWwiLCJpc3MiOiJodHRwczovL3Rlc3QubG9jYWwiLCJleHAiOjE3MTQ2NDAyODMsIm5iZiI6MTcxNDYzMzA4MywiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6InVzZXIiLCJncm91cCI6WyJncm91cDEiLCJncm91cDIiXSwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbInJvbGUxIiwicm9sZTIiXX0.gpi3rPV3MLKclzj7t9KqKAb5tgDehGIg2rg4U54OJIk";
            var provider = CreateProvider(null);

            // we expect this call to throw a SecurityTokenSignatureKeyNotFoundException because the signing key doesnt match
            await provider.ReadAndValidateTokenAsync(jwt);
        }

        [TestMethod]
        public async Task DecodeWithRightKeyAsync()
        {
            // this jwt was built with the provided key
            string jwt = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL3Rlc3QubG9jYWwiLCJpc3MiOiJodHRwczovL3Rlc3QubG9jYWwiLCJleHAiOjE3MTQ2NDAyODMsIm5iZiI6MTcxNDYzMzA4MywiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6InVzZXIiLCJncm91cCI6WyJncm91cDEiLCJncm91cDIiXSwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbInJvbGUxIiwicm9sZTIiXX0.gpi3rPV3MLKclzj7t9KqKAb5tgDehGIg2rg4U54OJIk";
            var provider = CreateProvider("ugjZEPvvIQpzmXG2TaAzEDRN6fNZC5IIRkf0CATs+gM=");

            // we expect this call to succeed since the signing key matches
            var validationResult = await provider.ReadAndValidateTokenAsync(jwt);

            // test that the token is extracted and contains the correct claims
            var name = validationResult.Claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] as string;
            var roles = validationResult.Claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] as IList<object>;

            Assert.IsNotNull(roles);
            Assert.AreEqual("user", name);
            Assert.IsTrue(roles.Any(x => string.Equals(x, "role1")));
            Assert.IsTrue(roles.Any(x => string.Equals(x, "role2")));
        }
    }
}