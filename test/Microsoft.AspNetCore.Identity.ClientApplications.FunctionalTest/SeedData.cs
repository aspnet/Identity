using System;
using System.Collections.Generic;
using System.Linq;
using Identity.ClientApplications.WebSite.Identity.Models;
using Microsoft.AspNetCore.Identity.Service;

namespace Microsoft.AspNetCore.Identity.ClientApplications.FunctionalTest
{
    public class ReferenceData
    {
        public IList<IdentityClientApplication> ClientApplications { get; set; } =
            new List<IdentityClientApplication>();

        public IList<(ApplicationUser user, string password)> UsersAndPasswords { get; set; } =
            new List<(ApplicationUser, string)>();

        public string DefaultUserName { get; private set; }

        public ReferenceData SetDefaultUserName(string name)
        {
            DefaultUserName = name;
            return this;
        }

        public ReferenceData CreateUser(string name, string password)
        {
            var user = new ApplicationUser()
            {
                UserName = name,
            };

            UsersAndPasswords.Add((user, password));

            return this;
        }

        public (ApplicationUser user, string password) GetUser(string name) =>
            UsersAndPasswords.Single(u => u.user.UserName == name);

        public (ApplicationUser user, string password) GetDefaultUser() =>
            DefaultUserName != null ? GetUser(DefaultUserName) : UsersAndPasswords.First();

        public ReferenceData CreateIntegratedWebClientApplication(string clientId)
        {
            return CreateApplication(
                        "IntegratedWebClient",
                        clientId,
                        "openid",
                        "urn:self:aspnet:identity:integrated",
                        "urn:self:aspnet:identity:integrated");
        }

        public ReferenceData CreateResourceApplication(string clientId, string name, params string[] scopes)
        {
            return CreateApplication(
                name,
                clientId,
                scopes,
                Array.Empty<string>(),
                Array.Empty<string>());
        }

        public ReferenceData CreateApplication(
                string name,
                string clientId,
                string scopes,
                string redirectUri,
                string logoutRedirectUri)
        {
            var app = CreateApplicationCore(name, clientId, scopes, redirectUri, logoutRedirectUri);
            ClientApplications.Add(app);

            return this;
        }

        private static IdentityClientApplication CreateApplicationCore(
            string name,
            string clientId,
            string scopes,
            string redirectUri,
            string logoutRedirectUris) =>
            CreateApplicationCore(
                name,
                clientId,
                scopes.Split(' '),
                new[] { redirectUri },
                new[] { logoutRedirectUris });

        public ReferenceData CreateApplication(string name, string clientId, IEnumerable<string> scopes, IEnumerable<string> redirectUris, IEnumerable<string> logoutRedirectUris)
        {
            var app = CreateApplicationCore(name, clientId, scopes, redirectUris, logoutRedirectUris);
            ClientApplications.Add(app);

            return this;
        }

        private static IdentityClientApplication CreateApplicationCore(string name, string clientId, IEnumerable<string> scopes, IEnumerable<string> redirectUris, IEnumerable<string> logoutRedirectUris)
        {
            var applicationId = Guid.NewGuid().ToString();
            return new IdentityClientApplication()
            {
                Id = applicationId,
                ClientId = clientId,
                Name = name,
                RedirectUris = redirectUris
                    .Select(ru =>
                        new IdentityClientApplicationRedirectUri<string>
                        {
                            Id = Guid.NewGuid().ToString(),
                            ApplicationId = applicationId,
                            IsLogout = false,
                            Value = ru
                        }).Concat(logoutRedirectUris.Select(lu =>
                            new IdentityClientApplicationRedirectUri<string>
                            {
                                Id = Guid.NewGuid().ToString(),
                                ApplicationId = applicationId,
                                IsLogout = false,
                                Value = lu
                            })).ToList(),
                Scopes = scopes.Select(s =>
                    new IdentityClientApplicationScope<string>
                    {
                        Id = Guid.NewGuid().ToString(),
                        ApplicationId = applicationId,
                        Value = s
                    }).ToList()
            };
        }
    }
}
