// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class ManagementTests : IClassFixture<ServerFactory>
    {
        public ManagementTests(ServerFactory serverFactory)
        {
            ServerFactory = serverFactory;
        }

        public ServerFactory ServerFactory { get; }

        [Fact]
        public async Task CanEnableTwoFactorAuthentication()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);

            // Act & Assert
            await UserStories.EnableTwoFactorAuthentication(index);
        }

        [Fact]
        public async Task CanConfirmEmail()
        {
            // Arrange
            var emails = new ContosoEmailSender();
            var server = ServerFactory.CreateServer(builder =>
                builder.ConfigureServices(s => s.SetupTestEmailSender(emails)));
            var client = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);
            var manageIndex = await UserStories.SendEmailConfirmationLinkAsync(index);

            // Act & Assert
            Assert.Equal(2, emails.SentEmails.Count);
            var email = emails.SentEmails[1];
            await UserStories.ConfirmEmailAsync(email, client);
        }

        [Fact]
        public async Task CanChangePassword()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            var server = ServerFactory.CreateServer(builder =>
                builder.ConfigureTestServices(s => s.SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme)));

            var client = ServerFactory.CreateDefaultClient(server);
            var newClient = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = "!Test.Password1";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);

            // Act 1
            var changedPassword = await UserStories.ChangePasswordAsync(index, "!Test.Password1", "!Test.Password2");

            // Assert 1
            // RefreshSignIn generates a new security stamp claim
            Assert.NotEqual(GetSecurityStampClaimValue(principals[0]), GetSecurityStampClaimValue(principals[1]));

            // Act 2
            await UserStories.LoginExistingUserAsync(newClient, userName, "!Test.Password2");

            // Assert 2
            // Signing in again with a different client uses the same security stamp claim
            Assert.Equal(GetSecurityStampClaimValue(principals[1]), GetSecurityStampClaimValue(principals[2]));
        }

        [Fact]
        public async Task CanSetPasswordWithExternalLogin()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            var server = ServerFactory.CreateServer(builder =>
                builder.ConfigureTestServices(s => s.SetupTestThirdPartyLogin()
                .SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme)));

            var client = ServerFactory.CreateDefaultClient(server);
            var newClient = ServerFactory.CreateDefaultClient(server);

            var guid = Guid.NewGuid();
            var userName = $"{guid}";
            var email = $"{guid}@example.com";

            // Act 1
            var index = await UserStories.RegisterNewUserWithSocialLoginAsync(client, userName, email);
            index = await UserStories.LoginWithSocialLoginAsync(newClient, userName);

            // Assert 1
            Assert.NotNull(GetAuthenticationMethodIdentifierClaimValue(principals[1]));

            // Act 2
            var changedPassword = await UserStories.SetPasswordAsync(index, "!Test.Password2");

            // Assert 2
            // RefreshSignIn uses the same AuthenticationMethod claim value
            Assert.Equal(GetAuthenticationMethodIdentifierClaimValue(principals[1]), GetAuthenticationMethodIdentifierClaimValue(principals[2]));
        }

        private string GetSecurityStampClaimValue(ClaimsPrincipal principal)
        {
            return principal.Identities.Single().Claims.Single(c => c.Type == "AspNet.Identity.SecurityStamp").Value;
        }

        private string GetAuthenticationMethodIdentifierClaimValue(ClaimsPrincipal principal)
        {
            return principal.Identities.Single().Claims.Single(c => c.Type == ClaimTypes.AuthenticationMethod).Value;
        }

        [Fact]
        public async Task CanDownloadPersonalData()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);

            // Act & Assert
            var jsonData = await UserStories.DownloadPersonalData(index, userName);
            Assert.Contains($"\"Id\":\"", jsonData);
            Assert.Contains($"\"UserName\":\"{userName}\"", jsonData);
            Assert.Contains($"\"Email\":\"{userName}\"", jsonData);
            Assert.Contains($"\"EmailConfirmed\":\"False\"", jsonData);
            Assert.Contains($"\"PhoneNumber\":\"null\"", jsonData);
            Assert.Contains($"\"PhoneNumberConfirmed\":\"False\"", jsonData);
            Assert.Contains($"\"TwoFactorEnabled\":\"False\"", jsonData);
        }

        [Fact]
        public async Task CanDeleteUser()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);

            // Act & Assert
            await UserStories.DeleteUser(index, password);
        }
    }
}
