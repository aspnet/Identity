// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Applications.Authentication;
using Microsoft.AspNetCore.Identity.Service.Session;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class LoginManagerTest
    {
        [Fact]
        public async Task LoginManager_ReturnsForbidden_IfPromptIsNoneAndUserIsNotLoggedIn()
        {
            // Arrange
            var loginContextProvider = new Mock<ILoginContextProvider>();
            loginContextProvider.Setup(s => s.GetLoginContextAsync())
                .ReturnsAsync(new LoginContext(new ClaimsPrincipal(), new ClaimsPrincipal()));

            var loginContextFactory = new Mock<ILoginFactory>();
            var message = new OpenIdConnectMessage
            {
                Prompt = PromptValues.None
            };

            var request = AuthorizationRequest.Valid(message, new RequestGrants());
            var manager = new LoginManager(loginContextProvider.Object, loginContextFactory.Object, new ProtocolErrorProvider());

            // Act
            var result = await manager.CanLogIn(request);

            // Assert
            Assert.Equal(LoginStatus.Forbidden, result.Status);
        }

        public static TheoryData<ClaimsPrincipal, ClaimsPrincipal> LoginManagerCanLoginLoginRequiredData =>
            new TheoryData<ClaimsPrincipal, ClaimsPrincipal>
            {
                { new ClaimsPrincipal(), new ClaimsPrincipal() },
                { CreateUser("userId"), CreateApplications("userId", "clientId") },
                { CreateUser(), CreateApplications("userId","clientId") },
                { CreateUser("userId"), CreateApplications("otherUserId","clientId") },
                { CreateUser("userId"), CreateApplications("userId","otherClientId") }
            };

        [Theory]
        [MemberData(nameof(LoginManagerCanLoginLoginRequiredData))]
        public async Task LoginManager_ReturnsLoginRequired_IfPromptIsMandatory(
            ClaimsPrincipal userPrincipal,
            ClaimsPrincipal applicationsPrincipal)
        {
            // Arrange
            var loginContextProvider = new Mock<ILoginContextProvider>();
            loginContextProvider.Setup(s => s.GetLoginContextAsync())
                .ReturnsAsync(new LoginContext(userPrincipal, applicationsPrincipal));

            var loginContextFactory = new Mock<ILoginFactory>();
            var message = new OpenIdConnectMessage
            {
                Prompt = PromptValues.Login,
                ClientId = "clientId"
            };

            var request = AuthorizationRequest.Valid(message, new RequestGrants());
            var manager = new LoginManager(loginContextProvider.Object, loginContextFactory.Object, new ProtocolErrorProvider());

            // Act
            var result = await manager.CanLogIn(request);

            // Assert
            Assert.Equal(LoginStatus.LoginRequired, result.Status);
        }

        [Fact]
        public async Task LoginManager_ReturnsLoginRequired_IfUserIsNotLoggedIn()
        {
            // Arrange
            var loginContextProvider = new Mock<ILoginContextProvider>();
            loginContextProvider.Setup(s => s.GetLoginContextAsync())
                .ReturnsAsync(new LoginContext(CreateUser(), CreateApplications()));

            var loginContextFactory = new Mock<ILoginFactory>();
            var message = new OpenIdConnectMessage
            {
                ClientId = "clientId"
            };

            var request = AuthorizationRequest.Valid(message, new RequestGrants());
            var manager = new LoginManager(loginContextProvider.Object, loginContextFactory.Object, new ProtocolErrorProvider());

            // Act
            var result = await manager.CanLogIn(request);

            // Assert
            Assert.Equal(LoginStatus.LoginRequired, result.Status);
        }

        public static TheoryData<ClaimsPrincipal, ClaimsPrincipal> LoginManagerCanLoginAuthorizedData =>
            new TheoryData<ClaimsPrincipal, ClaimsPrincipal>
            {
                { CreateUser("userId"), CreateApplications("userId", "clientId") },
                { CreateUser("userId"), CreateApplications("otherUserId","clientId") },
                { CreateUser("userId"), CreateApplications("userId","otherClientId") }
            };

        [Theory]
        [MemberData(nameof(LoginManagerCanLoginAuthorizedData))]
        public async Task LoginManager_ReturnsLoginAuthorized_IfUserIsLoggedIn(
            ClaimsPrincipal userPrincipal,
            ClaimsPrincipal applicationsPrincipal)
        {
            // Arrange
            var loginContextProvider = new Mock<ILoginContextProvider>();
            loginContextProvider.Setup(s => s.GetLoginContextAsync())
                .ReturnsAsync(new LoginContext(userPrincipal, applicationsPrincipal));

            var expectedUser = new ClaimsPrincipal(new ClaimsIdentity());
            var expectedApplication = new ClaimsPrincipal(new ClaimsIdentity());
            var loginContextFactory = new Mock<ILoginFactory>();

            loginContextFactory.Setup(s => s.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedUser);
            loginContextFactory.Setup(s => s.GetApplicationAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedApplication);

            var message = new OpenIdConnectMessage
            {
                ClientId = "clientId"
            };

            var request = AuthorizationRequest.Valid(message, new RequestGrants());
            var manager = new LoginManager(loginContextProvider.Object, loginContextFactory.Object, new ProtocolErrorProvider());

            // Act
            var result = await manager.CanLogIn(request);

            // Assert
            Assert.Equal(LoginStatus.Authorized, result.Status);
            Assert.Equal(expectedUser, result.User);
            Assert.Equal(expectedApplication, result.Application);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsLogoutToRedirect_AndAppendsState()
        {
            // Arrange
            var userPrincipal = CreateUser("userId");
            var applicationsPrincipal = CreateApplications("userId", "clientId");

            var loginContextProvider = new Mock<ILoginContextProvider>();
            loginContextProvider.Setup(s => s.GetLoginContextAsync())
                .ReturnsAsync(new LoginContext(userPrincipal, applicationsPrincipal));
            loginContextProvider
                .Setup(s => s.LogOutAsync(userPrincipal, It.Is<ClaimsPrincipal>(cp => cp.Identities.Any(i => i.HasClaim(ClaimTypes.NameIdentifier, "userId")))))
                .Returns(Task.CompletedTask);

            var loginContextFactory = new Mock<ILoginFactory>();

            var message = new OpenIdConnectMessage
            {
                ClientId = "clientId",
                State = "state"
            };

            var manager = new LoginManager(loginContextProvider.Object, loginContextFactory.Object, new ProtocolErrorProvider());

            // Act
            var result = await manager.LogOutAsync(LogoutRequest.Valid(message, "https://www.example.com/logout"));

            // Assert
            Assert.Equal(LogoutStatus.RedirectToLogoutUri, result.Status);
            Assert.Equal("https://www.example.com/logout?state=state", result.LogoutRedirect);
            loginContextProvider.VerifyAll();
        }

        [Fact]
        public async Task LoginAsync_Delegates()
        {
            // Arrange
            var userPrincipal = CreateUser("userId");
            var applicationsPrincipal = CreateApplications("userId", "clientId");

            var loginContextProvider = new Mock<ILoginContextProvider>();
            loginContextProvider
                .Setup(s => s.LogInAsync(userPrincipal, applicationsPrincipal))
                .Returns(Task.CompletedTask);

            var loginContextFactory = new Mock<ILoginFactory>();

            var message = new OpenIdConnectMessage
            {
                ClientId = "clientId",
            };

            var manager = new LoginManager(loginContextProvider.Object, loginContextFactory.Object, new ProtocolErrorProvider());

            // Act
            await manager.LogInAsync(userPrincipal,applicationsPrincipal);

            // Assert
            loginContextProvider.VerifyAll();
        }

        private static ClaimsPrincipal CreateApplications(string userId = null, params string[] applicationIds)
        {
            if (userId == null)
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new ClaimsPrincipal(
                applicationIds.Select(clientId => new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(TokenClaimTypes.ClientId, clientId),
                        new Claim(TokenClaimTypes.LogoutRedirectUri,"https://www.example.com/logout")
                    }, "Session")));
        }

        private static ClaimsPrincipal CreateUser(string userId = null)
        {
            if (userId == null)
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userId)
                }, "Login"));
        }
    }
}
