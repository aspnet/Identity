﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class ManagementTests : LoggedTest, IClassFixture<ServerFactory>
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
            var client = ServerFactory.CreateDefaultClient(LoggerFactory);

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
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
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
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
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
            AssertClaimsNotEqual(principals[0], principals[1], "AspNet.Identity.SecurityStamp");

            // Act 2
            await UserStories.LoginExistingUserAsync(newClient, userName, "!Test.Password2");

            // Assert 2
            // Signing in again with a different client uses the same security stamp claim
            AssertClaimsEqual(principals[1], principals[2], "AspNet.Identity.SecurityStamp");
        }

        [Fact]
        public async Task CanSetPasswordWithExternalLogin()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
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
            Assert.NotNull(principals[1].Identities.Single().Claims.Single(c => c.Type == ClaimTypes.AuthenticationMethod).Value);

            // Act 2
            await UserStories.SetPasswordAsync(index, "!Test.Password2");

            // Assert 2
            // RefreshSignIn uses the same AuthenticationMethod claim value
            AssertClaimsEqual(principals[1], principals[2], ClaimTypes.AuthenticationMethod);

            // Act & Assert 3
            // Can log in with the password set above
            await UserStories.LoginExistingUserAsync(ServerFactory.CreateDefaultClient(server), email, "!Test.Password2");
        }

        [Fact]
        public async Task CanRemoveExternalLogin()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
                builder.ConfigureTestServices(s => s.SetupTestThirdPartyLogin()
                .SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme)));

            var client = ServerFactory.CreateDefaultClient(server);

            var guid = Guid.NewGuid();
            var userName = $"{guid}";
            var email = $"{guid}@example.com";

            // Act
            var index = await UserStories.RegisterNewUserAsync(client, email, "!TestPassword1");
            var linkLogin = await UserStories.LinkExternalLoginAsync(index, email);
            await UserStories.RemoveExternalLoginAsync(linkLogin, email);

            // RefreshSignIn generates a new security stamp claim
            AssertClaimsNotEqual(principals[0], principals[1], "AspNet.Identity.SecurityStamp");
        }

        [Fact]
        public async Task CanResetAuthenticator()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
                builder.ConfigureTestServices(s => s.SetupTestThirdPartyLogin()
                .SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme)));

            var client = ServerFactory.CreateDefaultClient(server);
            var newClient = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            // Act
            var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
            var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn);
            var twoFactorKey = showRecoveryCodes.Context.AuthenticatorKey;

            // Use a new client to simulate a new browser session.
            var index = await UserStories.LoginExistingUser2FaAsync(newClient, userName, password, twoFactorKey);
            await UserStories.ResetAuthenticator(index);

            // RefreshSignIn generates a new security stamp claim
            AssertClaimsNotEqual(principals[1], principals[2], "AspNet.Identity.SecurityStamp");
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task CanDownloadPersonalData(bool twoFactor, bool social)
        {
            // Arrange
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
                builder.ConfigureTestServices(s => s.SetupTestThirdPartyLogin()));

            var client = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var guid = Guid.NewGuid();
            var email = userName;

            var index = social
                ? await UserStories.RegisterNewUserWithSocialLoginAsync(client, userName, email)
                : await UserStories.RegisterNewUserAsync(client, email, "!TestPassword1");

            if (twoFactor)
            {
                await UserStories.EnableTwoFactorAuthentication(index);
            }

            // Act & Assert
            var jsonData = await UserStories.DownloadPersonalData(index, userName);
            Assert.Contains($"\"Id\":\"", jsonData);
            Assert.Contains($"\"UserName\":\"{userName}\"", jsonData);
            Assert.Contains($"\"Email\":\"{userName}\"", jsonData);
            Assert.Contains($"\"EmailConfirmed\":\"False\"", jsonData);
            Assert.Contains($"\"PhoneNumber\":\"null\"", jsonData);
            Assert.Contains($"\"PhoneNumberConfirmed\":\"False\"", jsonData);
            Assert.Contains($"\"TwoFactorEnabled\":\"{twoFactor}\"", jsonData);
            Assert.Equal(twoFactor, jsonData.Contains($"\"Authenticator Key\":\""));
            Assert.Equal(social, jsonData.Contains($"\"Contoso external login provider key\":\"{userName}\""));
        }

        [Fact]
        public async Task GetOnDownloadPersonalData_ReturnsNotFound()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient(LoggerFactory);
            await UserStories.RegisterNewUserAsync(client);

            // Act
            var response = await client.GetAsync("/Identity/Account/Manage/DownloadPersonalData");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CanDeleteUser()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient(LoggerFactory);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);

            // Act & Assert
            await UserStories.DeleteUser(index, password);
        }

        private void AssertClaimsEqual(ClaimsPrincipal expectedPrincipal, ClaimsPrincipal actualPrincipal, string claimType)
        {
            var expectedPrincipalClaim = expectedPrincipal.Identities.Single().Claims.Single(c => c.Type == claimType).Value;
            var actualPrincipalClaim = actualPrincipal.Identities.Single().Claims.Single(c => c.Type == claimType).Value;
            Assert.Equal(expectedPrincipalClaim, actualPrincipalClaim);
        }

        private void AssertClaimsNotEqual(ClaimsPrincipal expectedPrincipal, ClaimsPrincipal actualPrincipal, string claimType)
        {
            var expectedPrincipalClaim = expectedPrincipal.Identities.Single().Claims.Single(c => c.Type == claimType).Value;
            var actualPrincipalClaim = actualPrincipal.Identities.Single().Claims.Single(c => c.Type == claimType).Value;
            Assert.NotEqual(expectedPrincipalClaim, actualPrincipalClaim);
        }
    }
}
