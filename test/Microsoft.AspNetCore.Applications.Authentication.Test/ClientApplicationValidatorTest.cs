// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Applications.Authentication;
using Microsoft.AspNetCore.Identity.Service.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class ClientApplicationValidatorTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ValidateClientIdAsync_ChecksThatTheClientIdExist(bool exists)
        {
            // Arrange
            var options = new ApplicationTokenOptions();
            var store = new Mock<IApplicationStore<IdentityClientApplication>>();
            store.Setup(s => s.FindByClientIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists ? new IdentityClientApplication() : null);

            var manager = new ApplicationManager<IdentityClientApplication>(
                Options.Create(new ApplicationOptions()),
                store.Object,
                Mock.Of<IPasswordHasher<IdentityClientApplication>>(),
                Array.Empty<IApplicationValidator<IdentityClientApplication>>(),
                Mock.Of<ILogger<ApplicationManager<IdentityClientApplication>>>(),
                new ApplicationErrorDescriber());

            var clientValidator = new ClientApplicationValidator<IdentityClientApplication>(
                Options.Create(options),
                GetLoginManager(),
                manager,
                new ProtocolErrorProvider());

            // Act
            var validation = await clientValidator.ValidateClientIdAsync("clientId");

            // Assert
            Assert.Equal(exists, validation);
        }

        [Fact]
        public async Task ValidateClientCredentialsAsync_DelegatesToApplicationManager()
        {
            // Arrange
            var options = new ApplicationTokenOptions();
            var store = new Mock<IApplicationStore<IdentityClientApplication>>();
            store.Setup(s => s.FindByClientIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IdentityClientApplication());
            store.As<IApplicationClientSecretStore<IdentityClientApplication>>()
                .Setup(s => s.HasClientSecretAsync(It.IsAny<IdentityClientApplication>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var manager = new ApplicationManager<IdentityClientApplication>(
                Options.Create(new ApplicationOptions()),
                store.Object,
                Mock.Of<IPasswordHasher<IdentityClientApplication>>(),
                Array.Empty<IApplicationValidator<IdentityClientApplication>>(),
                Mock.Of<ILogger<ApplicationManager<IdentityClientApplication>>>(),
                new ApplicationErrorDescriber());

            var clientValidator = new ClientApplicationValidator<IdentityClientApplication>(
                Options.Create(options),
                GetLoginManager(),
                manager,
                new ProtocolErrorProvider());

            // Act
            var validation = await clientValidator.ValidateClientCredentialsAsync("clientId", null);

            // Assert
            Assert.True(validation);
        }

        private LoginManager GetLoginManager() => new LoginManager(
                Mock.Of<ILoginContextProvider>(),
                Mock.Of<ILoginFactory>(),
                new ProtocolErrorProvider());
    }
}
