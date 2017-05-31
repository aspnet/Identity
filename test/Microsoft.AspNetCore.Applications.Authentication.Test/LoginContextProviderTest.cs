// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Session;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Applications.Authentication
{
    public class LoginContextProviderTest
    {
        [Fact]
        public async Task GetLoginContext_AuthenticatesWithLoginAndSessionPolicies()
        {
            // Arrange
            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), "Login"))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "Login")), "Login")));

            authenticationService
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), "Session"))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "Session")), "Session")));

            var factory = new Mock<ILoginFactory>();
            var policyProvider = new Mock<IAuthorizationPolicyProvider>();
            policyProvider.Setup(pp => pp.GetPolicyAsync(ApplicationsAuthenticationDefaults.LoginPolicyName))
                .ReturnsAsync(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Login")
                .RequireAuthenticatedUser()
                .Build());

            policyProvider.Setup(pp => pp.GetPolicyAsync(ApplicationsAuthenticationDefaults.SessionPolicyName))
                .ReturnsAsync(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Login")
                .AddAuthenticationSchemes("Session")
                .RequireAuthenticatedUser()
                .Build());

            var collection = new ServiceCollection();
            collection.AddSingleton(authenticationService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = collection.BuildServiceProvider();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(s => s.HttpContext).Returns(httpContext);

            var ctxProvider = new LoginContextProvider(
                factory.Object,
                policyProvider.Object,
                contextAccessor.Object,
                new ProtocolErrorProvider());

            // Act
            var context = await ctxProvider.GetLoginContextAsync();

            // Assert
            Assert.NotNull(context);
            var userIdentity = Assert.Single(context.User.Identities);
            Assert.True(userIdentity.IsAuthenticated);
            var applicationIdentity = Assert.Single(context.Applications.Identities);
            Assert.Equal("Session", applicationIdentity.AuthenticationType);
        }

        [Fact]
        public async Task LogoutAsync_LogoutsFromLoginAndSession()
        {
            // Arrange
            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService
                .Setup(s => s.SignOutAsync(It.IsAny<HttpContext>(), "Login", It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            authenticationService
                .Setup(s => s.SignOutAsync(It.IsAny<HttpContext>(), "Session", It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var factory = new Mock<ILoginFactory>();
            var policyProvider = new Mock<IAuthorizationPolicyProvider>();
            policyProvider.Setup(pp => pp.GetPolicyAsync(ApplicationsAuthenticationDefaults.LoginPolicyName))
                .ReturnsAsync(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Login")
                .RequireAuthenticatedUser()
                .Build());

            policyProvider.Setup(pp => pp.GetPolicyAsync(ApplicationsAuthenticationDefaults.SessionPolicyName))
                .ReturnsAsync(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Login")
                .AddAuthenticationSchemes("Session")
                .RequireAuthenticatedUser()
                .Build());

            var collection = new ServiceCollection();
            collection.AddSingleton(authenticationService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = collection.BuildServiceProvider();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(s => s.HttpContext).Returns(httpContext);

            var ctxProvider = new LoginContextProvider(
                factory.Object,
                policyProvider.Object,
                contextAccessor.Object,
                new ProtocolErrorProvider());

            // Act
            await ctxProvider.LogOutAsync(new ClaimsPrincipal(new ClaimsIdentity()), new ClaimsPrincipal(new ClaimsIdentity()));

            // Assert
            authenticationService.VerifyAll();
        }

        [Fact]
        public async Task LoginAsync_AddsApplicationIdentityWhenSigningIn()
        {
            // Arrange
            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), "Login"))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "Login")), "Login")));

            authenticationService
                .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), "Session"))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(TokenClaimTypes.ClientId,"clientId"),
                    new Claim(TokenClaimTypes.UserId,"userId"),
                    new Claim(TokenClaimTypes.LogoutRedirectUri,"https://www.example.com/logout")
                }, "Session")), "Session")));

            authenticationService
                .Setup(s => s.SignInAsync(
                    It.IsAny<HttpContext>(),
                    "Session",
                    It.Is<ClaimsPrincipal>(cp => cp.Identities.Count() == 2),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var factory = new Mock<ILoginFactory>();
            var policyProvider = new Mock<IAuthorizationPolicyProvider>();
            policyProvider.Setup(pp => pp.GetPolicyAsync(ApplicationsAuthenticationDefaults.LoginPolicyName))
                .ReturnsAsync(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Login")
                .RequireAuthenticatedUser()
                .Build());

            policyProvider.Setup(pp => pp.GetPolicyAsync(ApplicationsAuthenticationDefaults.SessionPolicyName))
                .ReturnsAsync(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Login")
                .AddAuthenticationSchemes("Session")
                .RequireAuthenticatedUser()
                .Build());

            var collection = new ServiceCollection();
            collection.AddSingleton(authenticationService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = collection.BuildServiceProvider();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(s => s.HttpContext).Returns(httpContext);

            var ctxProvider = new LoginContextProvider(
                factory.Object,
                policyProvider.Object,
                contextAccessor.Object,
                new ProtocolErrorProvider());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "userId") }, "Login"));
            var application = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(TokenClaimTypes.ClientId, "other"),
                    new Claim(TokenClaimTypes.LogoutRedirectUri, "https://www.example.com/other/logout") },
                "Session"));

            // Act
            await ctxProvider.LogInAsync(user, application);

            // Assert
            authenticationService.VerifyAll();
        }
    }
}
