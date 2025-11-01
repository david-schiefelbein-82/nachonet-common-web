using System.DirectoryServices.AccountManagement;
using Nachonet.Common.Web.ActiveDirectory.Errors;
using Nachonet.Common.Web.ActiveDirectory.Config;
using Microsoft.Extensions.Logging;

namespace Nachonet.Common.Web.ActiveDirectory
{
    public class ActiveDirectoryUserAuthenticator(ILogger<ActiveDirectoryUserAuthenticator> logger, IActiveDirectoryConfig config) : IActiveDirectoryUserAuthenticator
    {
        private readonly IActiveDirectoryConfig _config = config;
        private readonly ILogger<ActiveDirectoryUserAuthenticator> _logger = logger;

        public static string Capitalise(string name)
        {
            if (name.Length == 0)
                return name;

            return char.ToUpper(name[0]) + name[1..];
        }

        public ActiveDirectoryUser Login(string username, string password)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new ActiveDirectoryException("Active Directory Authentication is only supported on Windows");
            }

            var domainSeperator = username.LastIndexOf('\\');
            string domain = _config.Domain;

            if (domainSeperator >= 0)
            {
                domain = username[..domainSeperator];
                username = username[(domainSeperator + 1)..];
            }

            _logger.LogDebug("Attempting to authenticate user {username} on domain {domain}", domain, username);
            using (var context = new PrincipalContext(ContextType.Domain, domain))
            {
                bool valid = context.ValidateCredentials(username, password);
                _logger.LogDebug("{domain}\\{username} authenticated", domain, username);

                if (valid)
                {
                    var groups = new List<string>();
                    // find your user
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, username);

                    // if valid - grab its groups
                    if (userPrincipal != null)
                    {
                        username = userPrincipal.SamAccountName;
                        PrincipalSearchResult<Principal> searchResults = userPrincipal.GetAuthorizationGroups();

                        // iterate over all groups
                        foreach (Principal p in searchResults)
                        {
                            // make sure to add only group principals
                            if (p is GroupPrincipal gp)
                            {
                                groups.Add(gp.Name);
                            }
                        }

                        var adUser = new ActiveDirectoryUser(Capitalise(username), [.. groups]);
                        if (!string.IsNullOrWhiteSpace(userPrincipal.EmailAddress))
                            adUser.Email = userPrincipal.EmailAddress;

                        if (!string.IsNullOrWhiteSpace(userPrincipal.GivenName))
                            adUser.GivenName = userPrincipal.GivenName;

                        if (!string.IsNullOrWhiteSpace(userPrincipal.Surname))
                            adUser.Surname = userPrincipal.Surname;

                        if (!string.IsNullOrWhiteSpace(userPrincipal.Name))
                            adUser.PreferredName = userPrincipal.Name;

                        return adUser;
                    }
                    else
                    {
                        throw new ActiveDirectoryException("cannot find username " + username);
                    }
                }
            }

            throw new ActiveDirectoryException("Username or password invalid");
        }
    }
}
