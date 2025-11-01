using Nachonet.Common.Web.AppLocal.Errors;
using Nachonet.Common.Web.AppLocal.Config;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;


namespace Nachonet.Common.Web.AppLocal
{
    public class AppLocalUserAuthenticator(ILogger<AppLocalUserAuthenticator> logger, IAppLocalConfig config) : IAppLocalUserAuthenticator
    {
        private const int KeySize = 64;
        private const int Iterations = 350000;
        private static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private readonly IAppLocalConfig _config = config;
        private readonly ILogger<AppLocalUserAuthenticator> _logger = logger;

        public static string Capitalise(string name)
        {
            if (name.Length == 0)
                return name;

            return char.ToUpper(name[0]) + name[1..];
        }

        public static string HashPasword(string password, string salt)
        {
            var saltBinary = Convert.FromHexString(salt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                System.Text.Encoding.UTF8.GetBytes(password),
                saltBinary,
                Iterations,
                hashAlgorithm,
                KeySize);
            return Convert.ToHexString(hash);
        }

        public static string GenerateNewSalt()
        {
            var salt = RandomNumberGenerator.GetBytes(KeySize);
            return Convert.ToHexString(salt);
        }

        public IAppLocalUser Login(string username, string password)
        {
            var domainSeperator = username.LastIndexOf('\\');
            string domain = string.Empty;

            if (domainSeperator >= 0)
            {
                domain = username[..domainSeperator];
                username = username[(domainSeperator + 1)..];
            }

            _logger.LogDebug("Attempting to authenticate user {username} on domain {domain}", domain, username);
            var matchedUser = _config.Users.FirstOrDefault(x => string.Equals(x.Username, username, StringComparison.CurrentCultureIgnoreCase));

            // hash password with the user's unique salt, and compare the hash to the stored value
            if (matchedUser == null || !string.Equals(HashPasword(password, matchedUser.PasswordSalt), matchedUser.Password))
            {
                throw new BasicAuthException("Username or password invalid");
            }

            var basicUser = new AppLocalUser(Capitalise(username), matchedUser.Groups);
            if (!string.IsNullOrWhiteSpace(matchedUser.EmailAddress))
                basicUser.Email = matchedUser.EmailAddress;

            if (!string.IsNullOrWhiteSpace(matchedUser.GivenName))
                basicUser.GivenName = matchedUser.GivenName;

            if (!string.IsNullOrWhiteSpace(matchedUser.Surname))
                basicUser.Surname = matchedUser.Surname;

            if (!string.IsNullOrWhiteSpace(matchedUser.PreferredName))
                basicUser.PreferredName = matchedUser.PreferredName;

            return basicUser;
        }
    }
}
