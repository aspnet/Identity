// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.FunctionalTests.Flows;
using Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure;
using Microsoft.AspNetCore.Identity.FunctionalTests.Pages;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class LoginTests
    {
        [Fact]
        public async Task CanLogInWithAPreviouslyRegisteredUser()
        {
            // Arrange
            var server = ServerFactory.CreateDefaultServer();
            var client = ServerFactory.CreateDefaultClient(server);
            var newClient = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            // Act & Assert
            await UserStories.RegisterNewUserAsync(client, userName, password);

            // Use a new client to simulate a new browser session.
            await UserStories.LoginExistingUserAsync(newClient, userName, password);
        }

        [Fact]
        public async Task CanLogInWithTwoFactorAuthentication()
        {
            // Arrange
            var server = ServerFactory.CreateDefaultServer();
            var client = ServerFactory.CreateDefaultClient(server);
            var newClient = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
            var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn, twoFactorEnabled: false);

            var twoFactorKey = showRecoveryCodes.Context[EnableAuthenticator.AuthenticatorKey];

            // Act & Assert
            // Use a new client to simulate a new browser session.
            var loginWith2fa = await UserStories.LoginExistingUser2FaAsync(newClient, userName, password, twoFactorKey);
        }

        [Fact]
        public async Task CanLogInWithRecoveryCode()
        {
            // Arrange
            var server = ServerFactory.CreateDefaultServer();
            var client = ServerFactory.CreateDefaultClient(server);
            var newClient = ServerFactory.CreateDefaultClient(server);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
            var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn, twoFactorEnabled: false);

            var recoveryCode = showRecoveryCodes.Context[ShowRecoveryCodes.RecoveryCodes]
                .Split(' ')
                .First();

            // Act & Assert
            // Use a new client to simulate a new browser session.
            var loginWith2fa = await UserStories.LoginExistingUserRecoveryCodeAsync(newClient, userName, password, recoveryCode);
        }
    }
}
