// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Test
{
    public class SecurityStampTest
    {
        [Fact]
        public async Task OnValidatePrincipalThrowsWithEmptyServiceCollection()
        {
            var scheme = new IdentityOptions().Cookies.ApplicationCookieAuthenticationScheme;
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.RequestServices).Returns(new ServiceCollection().BuildServiceProvider());
            var id = new ClaimsPrincipal(new ClaimsIdentity(scheme));
            var ticket = new AuthenticationTicket(id, new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow }, scheme);
            var context = new CookieValidatePrincipalContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => SecurityStampValidator.ValidatePrincipalAsync(context));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task OnValidatePrincipalTestSuccess(bool isPersistent)
        {
            var user = new TestUser("test");
            var userManager = MockHelpers.MockUserManager<TestUser>();
            var claimsManager = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
            var options = new IdentityOptions { SecurityStampValidationInterval = TimeSpan.Zero };
            var httpContext = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            var id = new ClaimsIdentity(options.Cookies.ApplicationCookieAuthenticationScheme);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var principal = new ClaimsPrincipal(id);

            var properties = new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow, IsPersistent = isPersistent };
            var signInManager = new Mock<SignInManager<TestUser>>(userManager.Object,
                contextAccessor.Object, claimsManager.Object, options, null);
            signInManager.Setup(s => s.ValidateSecurityStampAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user).Verifiable();
            signInManager.Setup(s => s.CreateUserPrincipalAsync(user)).ReturnsAsync(principal).Verifiable();
            var services = new ServiceCollection();
            services.AddSingleton(options);
            services.AddSingleton(signInManager.Object);
            services.AddSingleton<ISecurityStampValidator>(new SecurityStampValidator<TestUser>(options, signInManager.Object));
            httpContext.Setup(c => c.RequestServices).Returns(services.BuildServiceProvider());

            var ticket = new AuthenticationTicket(principal, 
                properties, 
                options.Cookies.ApplicationCookieAuthenticationScheme);
            var context = new CookieValidatePrincipalContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
            Assert.NotNull(context.Properties);
            Assert.NotNull(context.Options);
            Assert.NotNull(context.Principal);
            await
                SecurityStampValidator.ValidatePrincipalAsync(context);
            Assert.NotNull(context.Principal);
            signInManager.VerifyAll();
        }

        [Fact]
        public async Task OnValidateIdentityRejectsWhenValidateSecurityStampFails()
        {
            var user = new TestUser("test");
            var userManager = MockHelpers.MockUserManager<TestUser>();
            var claimsManager = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
            var options = new IdentityOptions { SecurityStampValidationInterval = TimeSpan.Zero };
            var httpContext = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            var signInManager = new Mock<SignInManager<TestUser>>(userManager.Object,
                contextAccessor.Object, claimsManager.Object, options, null);
            signInManager.Setup(s => s.ValidateSecurityStampAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(null).Verifiable();
            var services = new ServiceCollection();
            services.AddSingleton(options);
            services.AddSingleton(signInManager.Object);
            services.AddSingleton<ISecurityStampValidator>(new SecurityStampValidator<TestUser>(options, signInManager.Object));
            httpContext.Setup(c => c.RequestServices).Returns(services.BuildServiceProvider());
            var id = new ClaimsIdentity(options.Cookies.ApplicationCookieAuthenticationScheme);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(id),
                new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow },
                options.Cookies.ApplicationCookieAuthenticationScheme);
            var context = new CookieValidatePrincipalContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
            Assert.NotNull(context.Properties);
            Assert.NotNull(context.Options);
            Assert.NotNull(context.Principal);
            await SecurityStampValidator.ValidatePrincipalAsync(context);
            Assert.Null(context.Principal);
            signInManager.VerifyAll();
        }

        [Fact]
        public async Task OnValidateIdentityRejectsWhenNoIssuedUtc()
        {
            var user = new TestUser("test");
            var httpContext = new Mock<HttpContext>();
            var userManager = MockHelpers.MockUserManager<TestUser>();
            var claimsManager = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
            var options = new IdentityOptions { SecurityStampValidationInterval = TimeSpan.Zero };
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            var signInManager = new Mock<SignInManager<TestUser>>(userManager.Object,
                contextAccessor.Object, claimsManager.Object, options, null);
            signInManager.Setup(s => s.ValidateSecurityStampAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(null).Verifiable();
            var services = new ServiceCollection();
            services.AddSingleton(options);
            services.AddSingleton(signInManager.Object);
            services.AddSingleton<ISecurityStampValidator>(new SecurityStampValidator<TestUser>(options, signInManager.Object));
            httpContext.Setup(c => c.RequestServices).Returns(services.BuildServiceProvider());
            var id = new ClaimsIdentity(options.Cookies.ApplicationCookieAuthenticationScheme);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(id),
                new AuthenticationProperties(),
                options.Cookies.ApplicationCookieAuthenticationScheme);
            var context = new CookieValidatePrincipalContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
            Assert.NotNull(context.Properties);
            Assert.NotNull(context.Options);
            Assert.NotNull(context.Principal);
            await SecurityStampValidator.ValidatePrincipalAsync(context);
            Assert.Null(context.Principal);
            signInManager.VerifyAll();
        }

        [Fact]
        public async Task OnValidateIdentityDoesNotRejectsWhenNotExpired()
        {
            var user = new TestUser("test");
            var httpContext = new Mock<HttpContext>();
            var userManager = MockHelpers.MockUserManager<TestUser>();
            var claimsManager = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
            var options = new IdentityOptions { SecurityStampValidationInterval = TimeSpan.FromDays(1) };
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            var signInManager = new Mock<SignInManager<TestUser>>(userManager.Object,
                contextAccessor.Object, claimsManager.Object, options, null);
            signInManager.Setup(s => s.ValidateSecurityStampAsync(It.IsAny<ClaimsPrincipal>())).Throws(new Exception("Shouldn't be called"));
            signInManager.Setup(s => s.SignInAsync(user, false, null)).Throws(new Exception("Shouldn't be called"));
            var services = new ServiceCollection();
            services.AddSingleton(options);
            services.AddSingleton(signInManager.Object);
            services.AddSingleton<ISecurityStampValidator>(new SecurityStampValidator<TestUser>(options, signInManager.Object));
            httpContext.Setup(c => c.RequestServices).Returns(services.BuildServiceProvider());
            var id = new ClaimsIdentity(options.Cookies.ApplicationCookieAuthenticationScheme);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(id),
                new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow },
                options.Cookies.ApplicationCookieAuthenticationScheme);
            var context = new CookieValidatePrincipalContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
            Assert.NotNull(context.Properties);
            Assert.NotNull(context.Options);
            Assert.NotNull(context.Principal);
            await SecurityStampValidator.ValidatePrincipalAsync(context);
            Assert.NotNull(context.Principal);
        }
    }
}