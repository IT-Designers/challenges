using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using SubmissionEvaluation.Server.Contracts.Interfaces;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Server.Classes.Authentication
{
    public class LdapAuthentication : IUserAuthentication
    {
        private ILogger logger;

        public LdapAuthentication()
        {
            logger = null;
        }

        public LdapAuthentication(ILogger logger)
        {
            SetLogger(logger);
        }

        public void SetLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public Dictionary<string, string> VerifyUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new Dictionary<string, string>();
            }

            var attributes = new[] {"uid", "sn", "givenName", "mail"};
            return GetUserAttributes(username, password, attributes);
        }


        private Dictionary<string, string> GetUserAttributes(string username, string password, string[] attributes)
        {
            try
            {
                using (var adsConn = new LdapConnection())
                {
                    adsConn.SecureSocketLayer = true;
                    adsConn.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, sslpolicyerrors) => true; // Ignore SSL Certificate errors
                    adsConn.Connect(Settings.Ldap.Ip, Settings.Ldap.Port);
                    adsConn.Bind(Settings.Ldap.AccessUser, Settings.Ldap.AccessPassword);

                    var filter = Settings.Ldap.UidAttribute + "=" + username;
                    var userOu = $"{Settings.Ldap.OrganizationalUnit},{Settings.Ldap.Domain}";
                    var results = adsConn.Search(userOu, LdapConnection.SCOPE_SUB, filter, null, false);
                    var attributeTable = new Dictionary<string, string>();
                    while (results.hasMore())
                    {
                        var result = results.next();
                        if (result.getAttribute(Settings.Ldap.UidAttribute)?.StringValue != username)
                        {
                            continue;
                        }

                        foreach (var attribute in attributes)
                        {
                            var readAttribute = attribute == "uid" ? Settings.Ldap.UidAttribute : attribute;
                            attributeTable.Add(attribute, result.getAttribute(readAttribute)?.StringValue);
                        }

                        adsConn.Bind(result.DN, password);

                        return attributeTable;
                    }

                    return new Dictionary<string, string>(); // Invalid password
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "LDAP Authentication failed");
            }

            return new Dictionary<string, string>();
        }
    }
}
