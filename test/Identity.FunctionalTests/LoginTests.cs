// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.DefaultUI.WebSite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class LoginTests : LoggedTest, IClassFixture<ServerFactory>
    {
        public LoginTests(ServerFactory serverFactory, ITestOutputHelper output) : base(output)
        {
            ServerFactory = serverFactory;
        }

        public ServerFactory ServerFactory { get; }

        [Fact]
        public async Task CanLogInWithAPreviouslyRegisteredUser()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                var server = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(sc => sc.AddSingleton(loggerFactory)));

                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                // Act & Assert
                await UserStories.RegisterNewUserAsync(client, userName, password);

                // Use a new client to simulate a new browser session.
                await UserStories.LoginExistingUserAsync(newClient, userName, password);
            }
        }

        [Fact]
        public async Task CanLogInWithTwoFactorAuthentication()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                var server = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(sc => sc.AddSingleton(loggerFactory)));

                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
                var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn);

                var twoFactorKey = showRecoveryCodes.Context.AuthenticatorKey;

                // Act & Assert
                // Use a new client to simulate a new browser session.
                await UserStories.LoginExistingUser2FaAsync(newClient, userName, password, twoFactorKey);
            }
        }

        [Fact]
        public async Task CanLogInWithTwoFactorAuthentication_WithGlobalAuthorizeFilter()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                var server = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(sc => sc.AddSingleton(loggerFactory)));
                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
                var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn);

                var twoFactorKey = showRecoveryCodes.Context.AuthenticatorKey;

                // Act & Assert
                // Use a new client to simulate a new browser session.
                await UserStories.LoginExistingUser2FaAsync(newClient, userName, password, twoFactorKey);
            }
        }

        [Fact]
        public async Task CanLogInWithRecoveryCode()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                var server = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(sc => sc.AddSingleton(loggerFactory)));

                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
                var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn);

                var recoveryCode = showRecoveryCodes.Context.RecoveryCodes.First();

                // Act & Assert
                // Use a new client to simulate a new browser session.
                await UserStories.LoginExistingUserRecoveryCodeAsync(newClient, userName, password, recoveryCode);
            }
        }

        [Fact]
        public async Task CanLogInWithRecoveryCode_WithGlobalAuthorizeFilter()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                void ConfigureTestServices(IServiceCollection services) =>
                    services.SetupGlobalAuthorizeFilter().AddSingleton(loggerFactory);

                var server = ServerFactory
                    .WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices));
                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);
                var showRecoveryCodes = await UserStories.EnableTwoFactorAuthentication(loggedIn);

                var recoveryCode = showRecoveryCodes.Context.RecoveryCodes.First();

                // Act & Assert
                // Use a new client to simulate a new browser session.
                await UserStories.LoginExistingUserRecoveryCodeAsync(newClient, userName, password, recoveryCode);
            }
        }

        [Fact]
        public async Task CannotLogInWithoutRequiredEmailConfirmation()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange

                var emailSender = new ContosoEmailSender();
                void ConfigureTestServices(IServiceCollection services) => services
                        .SetupTestEmailSender(emailSender)
                        .SetupEmailRequired()
                        .AddSingleton(loggerFactory);

                var server = ServerFactory.WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices));

                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);

                // Act & Assert
                // Use a new client to simulate a new browser session.
                await Assert.ThrowsAnyAsync<XunitException>(() => UserStories.LoginExistingUserAsync(newClient, userName, password));
            }
        }

        [Fact]
        public async Task CanLogInAfterConfirmingEmail()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                var emailSender = new ContosoEmailSender();
                void ConfigureTestServices(IServiceCollection services) => services
                    .SetupTestEmailSender(emailSender)
                    .SetupEmailRequired()
                    .AddSingleton(loggerFactory);

                var server = ServerFactory.WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices));

                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";

                var loggedIn = await UserStories.RegisterNewUserAsync(client, userName, password);

                // Act & Assert
                // Use a new client to simulate a new browser session.
                var email = Assert.Single(emailSender.SentEmails);
                await UserStories.ConfirmEmailAsync(email, newClient);

                await UserStories.LoginExistingUserAsync(newClient, userName, password);
            }
        }

        [Fact]
        public async Task CanLoginWithASocialLoginProvider()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                void ConfigureTestServices(IServiceCollection services) =>
                    services.SetupTestThirdPartyLogin()
                    .AddSingleton(loggerFactory);

                var server = ServerFactory.WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices));

                var client = server.CreateClient();
                var newClient = server.CreateClient();

                var guid = Guid.NewGuid();
                var userName = $"{guid}";
                var email = $"{guid}@example.com";

                // Act & Assert
                await UserStories.RegisterNewUserWithSocialLoginAsync(client, userName, email);
                await UserStories.LoginWithSocialLoginAsync(newClient, userName);
            }
        }

        [Fact]
        public async Task CanLogInAfterResettingThePassword()
        {
            using (StartLog(out var loggerFactory))
            {
                // Arrange
                var emailSender = new ContosoEmailSender();
                void ConfigureTestServices(IServiceCollection services) => services
                    .SetupTestEmailSender(emailSender)
                    .AddSingleton(loggerFactory);

                var server = ServerFactory.WithWebHostBuilder(whb => whb.ConfigureServices(ConfigureTestServices));

                var client = server.CreateClient();
                var resetPasswordClient = server.CreateClient();
                var newClient = server.CreateClient();

                var userName = $"{Guid.NewGuid()}@example.com";
                var password = $"!Test.Password1$";
                var newPassword = $"!New.Password1$";

                await UserStories.RegisterNewUserAsync(client, userName, password);
                var registrationEmail = Assert.Single(emailSender.SentEmails);
                await UserStories.ConfirmEmailAsync(registrationEmail, client);

                // Act & Assert
                await UserStories.ForgotPasswordAsync(resetPasswordClient, userName);
                Assert.Equal(2, emailSender.SentEmails.Count);
                var email = emailSender.SentEmails[1];
                await UserStories.ResetPasswordAsync(resetPasswordClient, email, userName, newPassword);
                await UserStories.LoginExistingUserAsync(newClient, userName, newPassword);
            }
        }
    }
}
