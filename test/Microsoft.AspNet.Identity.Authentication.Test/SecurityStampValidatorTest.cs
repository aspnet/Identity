// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Security;
using Microsoft.AspNet.Identity.Test;
using Microsoft.AspNet.Security;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Identity.Authentication.Test
{
    public class SecurityStampTest
    {
        //[Fact]
        //public async Task OnValidateIdentityNoBoomWithNullManagerTest()
        //{
        //    var httpContext = new Mock<HttpContext>();
        //    httpContext.Setup(c => c.ApplicationServices).Returns(new ServiceCollection().BuildServiceProvider());
        //    var id = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
        //    var ticket = new AuthenticationTicket(id, new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow });
        //    var context = new CookieValidateIdentityContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
        //    await
        //        SecurityStampValidator.OnValidateIdentity<IdentityUser>(TimeSpan.Zero)
        //            .Invoke(context);
        //    Assert.NotNull(context.Identity);
        //}

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task OnValidateIdentityTestSuccess(bool isPersistent)
        {
            var user = new IdentityUser("test");
            var httpContext = new Mock<HttpContext>();
            var userManager = MockHelpers.MockUserManager<IdentityUser>();
            var authManager = new Mock<IAuthenticationManager>();
            var claimsManager = new Mock<IClaimsIdentityFactory<IdentityUser>>();
            var signInManager = new Mock<SignInManager<IdentityUser>>(userManager.Object,
                authManager.Object, claimsManager.Object);
            signInManager.Setup(s => s.ValidateSecurityStamp(It.IsAny<ClaimsIdentity>(), user.Id)).ReturnsAsync(user).Verifiable();
            signInManager.Setup(s => s.SignInAsync(user, isPersistent)).Returns(Task.FromResult(0)).Verifiable();
            var services = new ServiceCollection();
            services.AddInstance(signInManager.Object);
            httpContext.Setup(c => c.ApplicationServices).Returns(services.BuildServiceProvider());
            var id = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            //id.AddClaim(new Claim(ClaimsIdentityOptions.DefaultSecurityStampClaimType, securityStamp));

            var ticket = new AuthenticationTicket(id, new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow, IsPersistent = isPersistent });
            var context = new CookieValidateIdentityContext(httpContext.Object, ticket, new CookieAuthenticationOptions());
            Assert.NotNull(context.Properties);
            Assert.NotNull(context.Options);
            Assert.NotNull(context.Identity);
            await
                SecurityStampValidator.OnValidateIdentity<IdentityUser>(TimeSpan.Zero).Invoke(context);
            Assert.NotNull(context.Identity);

            signInManager.VerifyAll();

            //// change stamp and make sure it fails
            //UnitTestHelper.IsSuccess(await manager.UpdateSecurityStampAsync(user.Id));
            //await
            //    SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser>, IdentityUser>(TimeSpan.Zero, SignIn)
            //        .Invoke(context);
            //Assert.Null(context.Identity);
        }

        //[Fact]
        //public async Task OnValidateRejectsUnknownUserIdentityTest()
        //{
        //    var owinContext = new OwinContext();
        //    await CreateManager(owinContext);
        //    var manager = owinContext.GetUserManager<UserManager<IdentityUser>>();
        //    var user = new IdentityUser("test");
        //    UnitTestHelper.IsSuccess(await manager.CreateAsync(user));
        //    var id = await SignIn(manager, user);
        //    UnitTestHelper.IsSuccess(await manager.DeleteAsync(user));
        //    var ticket = new AuthenticationTicket(id, new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow });
        //    var context = new CookieValidateIdentityContext(owinContext, ticket, new CookieAuthenticationOptions());
        //    await
        //        SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser>, IdentityUser>(TimeSpan.Zero, SignIn)
        //            .Invoke(context);
        //    Assert.Null(context.Identity);
        //}

        //[Fact]
        //public async Task OnValidateIdentityRejectsWithNoIssuedUtcTest()
        //{
        //    var owinContext = new OwinContext();
        //    await CreateManager(owinContext);
        //    var manager = owinContext.GetUserManager<UserManager<IdentityUser>>();
        //    var user = new IdentityUser("test");
        //    UnitTestHelper.IsSuccess(await manager.CreateAsync(user));
        //    var id = await SignIn(manager, user);
        //    var ticket = new AuthenticationTicket(id, new AuthenticationProperties());
        //    var context = new CookieValidateIdentityContext(owinContext, ticket, new CookieAuthenticationOptions());
        //    await
        //        SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser>, IdentityUser>(TimeSpan.Zero, SignIn)
        //            .Invoke(context);
        //    Assert.NotNull(context.Identity);
        //    Assert.Equal(user.Id, id.GetUserId());

        //    // change stamp does fail validation when no utc
        //    UnitTestHelper.IsSuccess(await manager.UpdateSecurityStampAsync(user.Id));
        //    await
        //        SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser>, IdentityUser>(TimeSpan.Zero, SignIn)
        //            .Invoke(context);
        //    Assert.Null(context.Identity);
        //}

        //[Fact]
        //public async Task OnValidateIdentityDoesNotRejectRightAwayTest()
        //{
        //    var owinContext = new OwinContext();
        //    await CreateManager(owinContext);
        //    var manager = owinContext.GetUserManager<UserManager<IdentityUser>>();
        //    var user = new IdentityUser("test");
        //    UnitTestHelper.IsSuccess(await manager.CreateAsync(user));
        //    var id = await SignIn(manager, user);
        //    var ticket = new AuthenticationTicket(id, new AuthenticationProperties { IssuedUtc = DateTimeOffset.UtcNow });
        //    var context = new CookieValidateIdentityContext(owinContext, ticket, new CookieAuthenticationOptions());

        //    // change stamp does not fail validation when not enough time elapsed
        //    UnitTestHelper.IsSuccess(await manager.UpdateSecurityStampAsync(user.Id));
        //    await
        //        SecurityStampValidator.OnValidateIdentity<UserManager<IdentityUser>, IdentityUser>(
        //            TimeSpan.FromDays(1), SignIn).Invoke(context);
        //    Assert.NotNull(context.Identity);
        //    Assert.Equal(user.Id, id.GetUserId());
        //}

        //private Task<ClaimsIdentity> SignIn(UserManager<IdentityUser> manager, IdentityUser user)
        //{
        //    return manager.ClaimsIdentityFactory.CreateAsync(manager, user, DefaultAuthenticationTypes.ApplicationCookie);
        //}

        //private async Task CreateManager(OwinContext context)
        //{
        //    var options = new IdentityFactoryOptions<UserManager<IdentityUser>>
        //    {
        //        Provider = new TestProvider(),
        //        DataProtectionProvider = new DpapiDataProtectionProvider()
        //    };
        //    var middleware =
        //        new IdentityFactoryMiddleware
        //            <UserManager<IdentityUser>, IdentityFactoryOptions<UserManager<IdentityUser>>>(null, options);
        //    await middleware.Invoke(context);
        //}

        //private class TestProvider : IdentityFactoryProvider<UserManager<IdentityUser>>
        //{
        //    public TestProvider()
        //    {
        //        OnCreate = ((options, context) =>
        //        {
        //            var manager =
        //                new UserManager<IdentityUser>(new UserStore<IdentityUser>(UnitTestHelper.CreateDefaultDb()));
        //            manager.UserValidator = new UserValidator<IdentityUser>(manager)
        //            {
        //                AllowOnlyAlphanumericUserNames = true,
        //                RequireUniqueEmail = false
        //            };
        //            if (options.DataProtectionProvider != null)
        //            {
        //                manager.UserTokenProvider =
        //                    new DataProtectorTokenProvider<IdentityUser>(
        //                        options.DataProtectionProvider.Create("ASP.NET Identity"));
        //            }
        //            return manager;
        //        });
        //        OnDispose = (options, manager) => { };
        //    }
        //}
    }
}