// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.FunctionalTests.Flows;
using Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure;
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
            await AuthenticationFlow.RegisterNewUserAsync(client, userName, password);

            // Use a new client to simulate a new browser session.
            await AuthenticationFlow.LoginExistingUserAsync(newClient, userName, password);
        }
    }
}
