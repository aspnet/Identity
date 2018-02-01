// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.FunctionalTests.Flows;
using Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class ManagementTests
    {
        [Fact]
        public async Task CanEnableTwoFactorAuthentication()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient();

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            var index = await AuthenticationFlow.RegisterNewUserAsync(client, userName, password);

            // Act & Assert
            var manage = await index.ClickManageLinkAsync();
            var twoFactor = await manage.ClickTwoFactorLinkAsync(twoFactorEnabled: false);
            var enableAuthenticator = await twoFactor.ClickEnableAuthenticatorLinkAsync();
            var shoCodes = await enableAuthenticator.SendValidCodeAsync();
        }
    }
}
