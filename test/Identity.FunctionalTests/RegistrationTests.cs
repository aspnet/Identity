﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class RegistrationTests : LoggedTest, IClassFixture<ServerFactory>
    {
        public RegistrationTests(ServerFactory serverFactory)
        {
            ServerFactory = serverFactory;
        }

        public ServerFactory ServerFactory { get; }

        [Fact]
        public async Task CanRegisterAUser()
        {
            // Arrange
            var client = ServerFactory.CreateDefaultClient(LoggerFactory);

            var userName = $"{Guid.NewGuid()}@example.com";
            var password = $"!Test.Password1$";

            // Act & Assert
            await UserStories.RegisterNewUserAsync(client, userName, password);
        }

        [Fact]
        public async Task CanRegisterWithASocialLoginProvider()
        {
            // Arrange
            var server = ServerFactory.CreateServer(LoggerFactory, builder =>
                builder.ConfigureServices(services => services.SetupTestThirdPartyLogin()));
            var client = ServerFactory.CreateDefaultClient(server);

            var guid = Guid.NewGuid();
            var userName = $"{guid}";
            var email = $"{guid}@example.com";

            // Act & Assert
            await UserStories.RegisterNewUserWithSocialLoginAsync(client, userName, email);
        }
    }
}
