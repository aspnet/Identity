﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public abstract class ManagementTests<TStartup, TContext> : IClassFixture<ServerFactory<TStartup, TContext>>
        where TStartup : class
        where TContext : DbContext
    {
        public ManagementTests(ServerFactory<TStartup, TContext> serverFactory)
        {
            ServerFactory = serverFactory;
        }

        public ServerFactory<TStartup, TContext> ServerFactory { get; }

        [Fact]
        public async Task CanEnableTwoFactorAuthentication()
        {
            // Arrange
            var client = ServerFactory
                .CreateClient();

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
            void ConfigureTestServices(IServiceCollection services) =>
                services.SetupTestEmailSender(emails);

            var client = ServerFactory
                .WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices))
                .CreateClient();

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
        public async Task CanChangeEmail()
        {
            // Arrange
            var emails = new ContosoEmailSender();
            var client = ServerFactory
                .CreateClient();

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";
            var newEmail = "updatedEmail@example.com";

            var index = await UserStories.RegisterNewUserAsync(client, userName, password);
            var manageIndex = await UserStories.SendUpdateProfileAsync(index, newEmail);

            // Act & Assert
            var pageUserName = manageIndex.GetUserName();
            Assert.Equal(newEmail, pageUserName);
            var pageEmail = manageIndex.GetEmail();
            Assert.Equal(newEmail, pageEmail);
        }

        [Fact]
        public async Task CanChangePassword()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            void ConfigureTestServices(IServiceCollection services) =>
                services.SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme);

            var server = ServerFactory
                .WithWebHostBuilder(whb => whb.ConfigureTestServices(ConfigureTestServices));

            var client = server.CreateClient();
            var newClient = server.CreateClient();

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
            void ConfigureTestServices(IServiceCollection services) =>
                services
                    .SetupTestThirdPartyLogin()
                    .SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme);

            var server = ServerFactory
                .WithWebHostBuilder(whb => whb.ConfigureTestServices(ConfigureTestServices));

            var client = server.CreateClient();
            var newClient = server.CreateClient();
            var loginAfterSetPasswordClient = server.CreateClient();

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
            await UserStories.LoginExistingUserAsync(loginAfterSetPasswordClient, email, "!Test.Password2");
        }

        [Fact]
        public async Task CanRemoveExternalLogin()
        {
            // Arrange
            var principals = new List<ClaimsPrincipal>();
            void ConfigureTestServices(IServiceCollection services) =>
                services
                    .SetupTestThirdPartyLogin()
                    .SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme);

            var server = ServerFactory
                .WithWebHostBuilder(whb => whb.ConfigureTestServices(ConfigureTestServices));

            var client = server.CreateClient();

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
            void ConfigureTestServices(IServiceCollection services) =>
                services
                    .SetupTestThirdPartyLogin()
                    .SetupGetUserClaimsPrincipal(user => principals.Add(user), IdentityConstants.ApplicationScheme);

            var server = ServerFactory
                .WithWebHostBuilder(whb => whb.ConfigureTestServices(ConfigureTestServices));

            var client = server.CreateClient();
            var newClient = server.CreateClient();

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
            void ConfigureTestServices(IServiceCollection services) =>
                services.SetupTestThirdPartyLogin();

            var client = ServerFactory
                .WithWebHostBuilder(whb => whb.ConfigureTestServices(ConfigureTestServices))
                .CreateClient();

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
            Assert.NotNull(jsonData);
            Assert.True(jsonData.ContainsKey("Id"));
            Assert.NotNull(jsonData["Id"]);
            Assert.True(jsonData.ContainsKey("UserName"));
            Assert.Equal(userName, (string)jsonData["UserName"]);
            Assert.True(jsonData.ContainsKey("Email"));
            Assert.Equal(userName, (string)jsonData["Email"]);
            Assert.True(jsonData.ContainsKey("EmailConfirmed"));
            Assert.False((bool)jsonData["EmailConfirmed"]);
            Assert.True(jsonData.ContainsKey("PhoneNumber"));
            Assert.Equal("null", (string)jsonData["PhoneNumber"]);
            Assert.True(jsonData.ContainsKey("PhoneNumberConfirmed"));
            Assert.False((bool)jsonData["PhoneNumberConfirmed"]);
            Assert.Equal(twoFactor, (bool)jsonData["TwoFactorEnabled"]);

            if (twoFactor)
            {
                Assert.NotNull(jsonData["Authenticator Key"]);
            }
            else
            {
                Assert.Null((string)jsonData["Authenticator Key"]);
            }

            if (social)
            {
                Assert.Equal(userName, (string)jsonData["Contoso external login provider key"]);
            }
            else
            {
                Assert.Null((string)jsonData["Contoso external login provider key"]);
            }
        }

        [Fact]
        public async Task GetOnDownloadPersonalData_ReturnsNotFound()
        {
            // Arrange
            var client = ServerFactory
                .CreateClient();

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
            var client = ServerFactory
                .CreateClient();

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
