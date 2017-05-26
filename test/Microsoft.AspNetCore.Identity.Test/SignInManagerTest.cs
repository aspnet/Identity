// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Test
{
    public class SignManagerInTest
    {
        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async Task VerifyAccountControllerSignInFunctional(bool isPersistent)
        //{
        //    var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
        //    app.UseCookieAuthentication(new CookieAuthenticationOptions
        //    {
        //        AuthenticationScheme = ClaimsIdentityOptions.DefaultAuthenticationScheme
        //    });

        // TODO: how to functionally test context?
        //    var context = new DefaultHttpContext(new FeatureCollection());
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(a => a.Value).Returns(context);
        //    app.UseServices(services =>
        //    {
        //        services.AddSingleton(contextAccessor.Object);
        //        services.AddSingleton<Ilogger>(new Nulllogger());
        //        services.AddIdentity<ApplicationUser, IdentityRole>(s =>
        //        {
        //            s.AddUserStore(() => new InMemoryUserStore<ApplicationUser>());
        //            s.AddUserManager<ApplicationUserManager>();
        //            s.AddRoleStore(() => new InMemoryRoleStore<IdentityRole>());
        //            s.AddRoleManager<ApplicationRoleManager>();
        //        });
        //        services.AddTransient<ApplicationHttpSignInManager>();
        //    });

        //    // Act
        //    var user = new ApplicationUser
        //    {
        //        UserName = "Yolo"
        //    };
        //    const string password = "Yol0Sw@g!";
        //    var userManager = app.ApplicationServices.GetRequiredService<ApplicationUserManager>();
        //    var HttpSignInManager = app.ApplicationServices.GetRequiredService<ApplicationHttpSignInManager>();

        //    IdentityResultAssert.IsSuccess(await userManager.CreateAsync(user, password));
        //    var result = await HttpSignInManager.PasswordSignInAsync(user.UserName, password, isPersistent, false);

        //    // Assert
        //    Assert.Equal(SignInStatus.Success, result);
        //    contextAccessor.Verify();
        //}

        [Fact]
        public void ConstructorNullChecks()
        {
            Assert.Throws<ArgumentNullException>("userManager", () => new SignInManager<TestUser>(null, null, null, null, null, null));
            var userManager = MockHelpers.MockUserManager<TestUser>().Object;
            Assert.Throws<ArgumentNullException>("contextAccessor", () => new SignInManager<TestUser>(userManager, null, null, null, null, null));
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var context = new Mock<HttpContext>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);
            Assert.Throws<ArgumentNullException>("claimsFactory", () => new SignInManager<TestUser>(userManager, contextAccessor.Object, null, null, null, null));
        }

        //[Fact]
        //public async Task EnsureClaimsPrincipalFactoryCreateIdentityCalled()
        //{
        //    // Setup
        //    var user = new TestUser { UserName = "Foo" };
        //    var userManager = MockHelpers.TestUserManager<TestUser>();
        //    var identityFactory = new Mock<IUserClaimsPrincipalFactory<TestUser>>();
        //    const string authType = "Test";
        //    var testIdentity = new ClaimsPrincipal();
        //    identityFactory.Setup(s => s.CreateAsync(user)).ReturnsAsync(testIdentity).Verifiable();
        //    var context = new Mock<HttpContext>();
        //    var response = new Mock<HttpResponse>();
        //    context.Setup(c => c.Response).Returns(response.Object).Verifiable();
        //    response.Setup(r => r.SignIn(testIdentity, It.IsAny<AuthenticationProperties>())).Verifiable();
        //    var contextAccessor = new Mock<IHttpContextAccessor>();
        //    contextAccessor.Setup(a => a.Value).Returns(context.Object);
        //    var helper = new HttpAuthenticationManager(contextAccessor.Object);

        //    // Act
        //    helper.SignIn(user, false);

        //    // Assert
        //    identityFactory.Verify();
        //    context.Verify();
        //    contextAccessor.Verify();
        //    response.Verify();
        //}

        [Fact]
        public async Task PasswordSignInReturnsLockedOutWhenLockedOut()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(true).Verifiable();

            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);
            var roleManager = MockHelpers.MockRoleManager<TestRole>();
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TestUser, TestRole>(manager.Object, roleManager.Object, options.Object);
            var logStore = new StringBuilder();
            var logger = MockHelpers.MockILogger<SignInManager<TestUser>>(logStore);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object, claimsFactory, options.Object, logger.Object, new Mock<IAuthenticationSchemeProvider>().Object);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "bogus", false, false);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsLockedOut);
            Assert.True(logStore.ToString().Contains($"User {user.Id} is currently locked out."));
            manager.Verify();
        }

        [Fact]
        public async Task CheckPasswordSignInReturnsLockedOutWhenLockedOut()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(true).Verifiable();

            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context.Object);
            var roleManager = MockHelpers.MockRoleManager<TestRole>();
            var identityOptions = new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TestUser, TestRole>(manager.Object, roleManager.Object, options.Object);
            var logStore = new StringBuilder();
            var logger = MockHelpers.MockILogger<SignInManager<TestUser>>(logStore);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object, claimsFactory, options.Object, logger.Object, new Mock<IAuthenticationSchemeProvider>().Object);

            // Act
            var result = await helper.CheckPasswordSignInAsync(user, "bogus", false);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsLockedOut);
            Assert.True(logStore.ToString().Contains($"User {user.Id} is currently locked out."));
            manager.Verify();
        }

        private static Mock<UserManager<TestUser>> SetupUserManager(TestUser user)
        {
            var manager = MockHelpers.MockUserManager<TestUser>();
            manager.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);
            manager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            manager.Setup(m => m.GetUserIdAsync(user)).ReturnsAsync(user.Id.ToString());
            manager.Setup(m => m.GetUserNameAsync(user)).ReturnsAsync(user.UserName);
            return manager;
        }

        private static SignInManager<TestUser> SetupSignInManager(UserManager<TestUser> manager, HttpContext context, StringBuilder logStore = null, IdentityOptions identityOptions = null)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context);
            var roleManager = MockHelpers.MockRoleManager<TestRole>();
            identityOptions = identityOptions ?? new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TestUser, TestRole>(manager, roleManager.Object, options.Object);
            var sm = new SignInManager<TestUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, new Mock<IAuthenticationSchemeProvider>().Object);
            sm.Logger = MockHelpers.MockILogger<SignInManager<TestUser>>(logStore ?? new StringBuilder()).Object;
            return sm;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanPasswordSignIn(bool isPersistent)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
            manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();

            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            SetupSignIn(context, auth, user.Id, isPersistent);
            var helper = SetupSignInManager(manager.Object, context);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "password", isPersistent, false);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        [Fact]
        public async Task CanPasswordSignInWithNoLogger()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
            manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();

            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            SetupSignIn(context, auth, user.Id, false);
            var helper = SetupSignInManager(manager.Object, context);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "password", false, false);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }


        [Fact]
        public async Task PasswordSignInWorksWithNonTwoFactorStore()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
            manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();

            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            SetupSignIn(context, auth);
            var helper = SetupSignInManager(manager.Object, context);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "password", false, false);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task PasswordSignInRequiresVerification(bool supportsLockout)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(supportsLockout).Verifiable();
            if (supportsLockout)
            {
                manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            }
            IList<string> providers = new List<string>();
            providers.Add("PhoneNumber");
            manager.Setup(m => m.GetValidTwoFactorProvidersAsync(user)).Returns(Task.FromResult(providers)).Verifiable();
            manager.Setup(m => m.SupportsUserTwoFactor).Returns(true).Verifiable();
            manager.Setup(m => m.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
            if (supportsLockout)
            {
                manager.Setup(m => m.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            }
            var context = new DefaultHttpContext();
            var helper = SetupSignInManager(manager.Object, context);
            var auth = MockAuth(context);
            auth.Setup(a => a.SignInAsync(context, IdentityConstants.TwoFactorUserIdScheme,
                It.Is<ClaimsPrincipal>(id => id.FindFirstValue(ClaimTypes.Name) == user.Id),
                It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "password", false, false);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.RequiresTwoFactor);
            manager.Verify();
            auth.Verify();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExternalSignInRequiresVerificationIfNotBypassed(bool bypass)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            const string loginProvider = "login";
            const string providerKey = "fookey";
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(false).Verifiable();
            manager.Setup(m => m.FindByLoginAsync(loginProvider, providerKey)).ReturnsAsync(user).Verifiable();
            if (!bypass)
            {
                IList<string> providers = new List<string>();
                providers.Add("PhoneNumber");
                manager.Setup(m => m.GetValidTwoFactorProvidersAsync(user)).Returns(Task.FromResult(providers)).Verifiable();
                manager.Setup(m => m.SupportsUserTwoFactor).Returns(true).Verifiable();
                manager.Setup(m => m.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true).Verifiable();
            }
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var helper = SetupSignInManager(manager.Object, context);

            if (bypass)
            {
                SetupSignIn(context, auth, user.Id, false, loginProvider);
            }
            else
            {
                auth.Setup(a => a.SignInAsync(context, IdentityConstants.TwoFactorUserIdScheme,
                    It.Is<ClaimsPrincipal>(id => id.FindFirstValue(ClaimTypes.Name) == user.Id),
                    It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            }

            // Act
            var result = await helper.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent: false, bypassTwoFactor: bypass);

            // Assert
            Assert.Equal(bypass, result.Succeeded);
            Assert.Equal(!bypass, result.RequiresTwoFactor);
            manager.Verify();
            auth.Verify();
        }

        private class GoodTokenProvider : AuthenticatorTokenProvider<TestUser>
        {
            public override Task<bool> ValidateAsync(string purpose, string token, UserManager<TestUser> manager, TestUser user)
            {
                return Task.FromResult(true);
            }
        }


        [Theory]
        [InlineData(null, true, true)]
        [InlineData("Authenticator", false, true)]
        [InlineData("Gooblygook", true, false)]
        [InlineData("--", false, false)]
        public async Task CanTwoFactorAuthenticatorSignIn(string providerName, bool isPersistent, bool rememberClient)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            const string code = "3123";
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.VerifyTwoFactorTokenAsync(user, providerName ?? TokenOptions.DefaultAuthenticatorProvider, code)).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
            manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();

            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var helper = SetupSignInManager(manager.Object, context);
            var twoFactorInfo = new SignInManager<TestUser>.TwoFactorAuthenticationInfo { UserId = user.Id };
            if (providerName != null)
            {
                helper.Options.Tokens.AuthenticatorTokenProvider = providerName;
            }
            var id = helper.StoreTwoFactorInfo(user.Id, null);
            SetupSignIn(context, auth, user.Id, isPersistent);
            auth.Setup(a => a.AuthenticateAsync(context, IdentityConstants.TwoFactorUserIdScheme))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(id, null, IdentityConstants.TwoFactorUserIdScheme))).Verifiable();
            if (rememberClient)
            {
                auth.Setup(a => a.SignInAsync(context,
                    IdentityConstants.TwoFactorRememberMeScheme,
                    It.Is<ClaimsPrincipal>(i => i.FindFirstValue(ClaimTypes.Name) == user.Id
                        && i.Identities.First().AuthenticationType == IdentityConstants.TwoFactorRememberMeScheme),
                    It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            }

            // Act
            var result = await helper.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task CanTwoFactorRecoveryCodeSignIn(bool supportsLockout, bool externalLogin)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            const string bypassCode = "someCode";
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(supportsLockout).Verifiable();
            manager.Setup(m => m.RedeemTwoFactorRecoveryCodeAsync(user, bypassCode)).ReturnsAsync(IdentityResult.Success).Verifiable();
            manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
            manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            if (supportsLockout)
            {
                manager.Setup(m => m.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            }
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var helper = SetupSignInManager(manager.Object, context);
            var twoFactorInfo = new SignInManager<TestUser>.TwoFactorAuthenticationInfo { UserId = user.Id };
            var loginProvider = "loginprovider";
            var id = helper.StoreTwoFactorInfo(user.Id, externalLogin ? loginProvider : null);
            if (externalLogin)
            {
                auth.Setup(a => a.SignInAsync(context,
                    IdentityConstants.ApplicationScheme,
                    It.Is<ClaimsPrincipal>(i => i.FindFirstValue(ClaimTypes.AuthenticationMethod) == loginProvider
                        && i.FindFirstValue(ClaimTypes.NameIdentifier) == user.Id),
                    It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
                auth.Setup(a => a.SignOutAsync(context, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
                auth.Setup(a => a.SignOutAsync(context, IdentityConstants.TwoFactorUserIdScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            }
            else
            {
                SetupSignIn(context, auth, user.Id);
            }
            auth.Setup(a => a.AuthenticateAsync(context, IdentityConstants.TwoFactorUserIdScheme))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(id, null, IdentityConstants.TwoFactorUserIdScheme))).Verifiable();

            // Act
            var result = await helper.TwoFactorRecoveryCodeSignInAsync(bypassCode);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public async Task CanExternalSignIn(bool isPersistent, bool supportsLockout, bool supportsActivity)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            const string loginProvider = "login";
            const string providerKey = "fookey";
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(supportsLockout).Verifiable();
            if (supportsLockout)
            {
                manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            }
            manager.Setup(m => m.FindByLoginAsync(loginProvider, providerKey)).ReturnsAsync(user).Verifiable();
            if (supportsActivity)
            {
                manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
                manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            }

            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var helper = SetupSignInManager(manager.Object, context);
            SetupSignIn(context, auth, user.Id, isPersistent, loginProvider);

            // Act
            var result = await helper.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task CanResignIn(bool isPersistent, bool externalLogin)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var loginProvider = "loginprovider";
            var id = new ClaimsIdentity();
            if (externalLogin)
            {
                id.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }
            // REVIEW: auth changes we lost the ability to mock is persistent
            //var properties = new AuthenticationProperties { IsPersistent = isPersistent };
            var authResult = AuthenticateResult.None();
            auth.Setup(a => a.AuthenticateAsync(context, IdentityConstants.ApplicationScheme))
                .Returns(Task.FromResult(authResult)).Verifiable();
            var manager = SetupUserManager(user);
            var signInManager = new Mock<SignInManager<TestUser>>(manager.Object,
                new HttpContextAccessor { HttpContext = context },
                new Mock<IUserClaimsPrincipalFactory<TestUser>>().Object,
                null, null, new Mock<IAuthenticationSchemeProvider>().Object)
            { CallBase = true };
            //signInManager.Setup(s => s.SignInAsync(user, It.Is<AuthenticationProperties>(p => p.IsPersistent == isPersistent),
            //externalLogin? loginProvider : null)).Returns(Task.FromResult(0)).Verifiable();
            signInManager.Setup(s => s.SignInAsync(user, It.IsAny<AuthenticationProperties>(), /*authmethod*/null, /*updateLastSignIn*/false)).Returns(Task.FromResult(0)).Verifiable();

            // Act
            await signInManager.Object.RefreshSignInAsync(user);

            // Assert
            auth.Verify();
            signInManager.Verify();
        }

        [Theory]
        [InlineData(true, true, true, true, true)]
        [InlineData(true, true, false, true, true)]
        [InlineData(true, false, true, true, true)]
        [InlineData(true, false, false, true, true)]
        [InlineData(false, true, true, true, true)]
        [InlineData(false, true, false, true, true)]
        [InlineData(false, false, true, true, true)]
        [InlineData(false, false, false, true, true)]
        [InlineData(true, true, true, false, true)]
        [InlineData(true, true, false, false, true)]
        [InlineData(true, false, true, false, true)]
        [InlineData(true, false, false, false, true)]
        [InlineData(false, true, true, false, true)]
        [InlineData(false, true, false, false, true)]
        [InlineData(false, false, true, false, true)]
        [InlineData(false, false, false, false, true)]
        [InlineData(true, true, true, true, false)]
        [InlineData(true, true, false, true, false)]
        [InlineData(true, false, true, true, false)]
        [InlineData(true, false, false, true, false)]
        [InlineData(false, true, true, true, false)]
        [InlineData(false, true, false, true, false)]
        [InlineData(false, false, true, true, false)]
        [InlineData(false, false, false, true, false)]
        [InlineData(true, true, true, false, false)]
        [InlineData(true, true, false, false, false)]
        [InlineData(true, false, true, false, false)]
        [InlineData(true, false, false, false, false)]
        [InlineData(false, true, true, false, false)]
        [InlineData(false, true, false, false, false)]
        [InlineData(false, false, true, false, false)]
        [InlineData(false, false, false, false, false)]
        public async Task CanTwoFactorSignIn(bool isPersistent, bool supportsLockout, bool externalLogin, bool rememberClient, bool supportsUserActivity)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            var provider = "twofactorprovider";
            var code = "123456";
            manager.Setup(m => m.SupportsUserLockout).Returns(supportsLockout).Verifiable();
            if (supportsLockout)
            {
                manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
                manager.Setup(m => m.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            }
            if (supportsUserActivity)
            {
                manager.Setup(m => m.SupportsUserActivity).Returns(true).Verifiable();
                manager.Setup(m => m.UpdateLastSignInDateAsync(user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            }
            manager.Setup(m => m.VerifyTwoFactorTokenAsync(user, provider, code)).ReturnsAsync(true).Verifiable();
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var helper = SetupSignInManager(manager.Object, context);
            var twoFactorInfo = new SignInManager<TestUser>.TwoFactorAuthenticationInfo { UserId = user.Id };
            var loginProvider = "loginprovider";
            var id = helper.StoreTwoFactorInfo(user.Id, externalLogin ? loginProvider : null);
            if (externalLogin)
            {
                auth.Setup(a => a.SignInAsync(context,
                    IdentityConstants.ApplicationScheme,
                    It.Is<ClaimsPrincipal>(i => i.FindFirstValue(ClaimTypes.AuthenticationMethod) == loginProvider
                        && i.FindFirstValue(ClaimTypes.NameIdentifier) == user.Id),
                    It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
                // REVIEW: restore ability to test is persistent
                //It.Is<AuthenticationProperties>(v => v.IsPersistent == isPersistent))).Verifiable();
                auth.Setup(a => a.SignOutAsync(context, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
                auth.Setup(a => a.SignOutAsync(context, IdentityConstants.TwoFactorUserIdScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            }
            else
            {
                SetupSignIn(context, auth, user.Id);
            }
            if (rememberClient)
            {
                auth.Setup(a => a.SignInAsync(context,
                    IdentityConstants.TwoFactorRememberMeScheme,
                    It.Is<ClaimsPrincipal>(i => i.FindFirstValue(ClaimTypes.Name) == user.Id
                        && i.Identities.First().AuthenticationType == IdentityConstants.TwoFactorRememberMeScheme),
                    It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
                //It.Is<AuthenticationProperties>(v => v.IsPersistent == true))).Returns(Task.FromResult(0)).Verifiable();
            }
            auth.Setup(a => a.AuthenticateAsync(context, IdentityConstants.TwoFactorUserIdScheme))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(id, null, IdentityConstants.TwoFactorUserIdScheme))).Verifiable();

            // Act
            var result = await helper.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        [Fact]
        public async Task RememberClientStoresUserId()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            var helper = SetupSignInManager(manager.Object, context);
            auth.Setup(a => a.SignInAsync(
                context,
                IdentityConstants.TwoFactorRememberMeScheme,
                It.Is<ClaimsPrincipal>(i => i.FindFirstValue(ClaimTypes.Name) == user.Id
                    && i.Identities.First().AuthenticationType == IdentityConstants.TwoFactorRememberMeScheme),
                It.Is<AuthenticationProperties>(v => v.IsPersistent == true))).Returns(Task.FromResult(0)).Verifiable();


            // Act
            await helper.RememberTwoFactorClientAsync(user);

            // Assert
            manager.Verify();
            auth.Verify();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RememberBrowserSkipsTwoFactorVerificationSignIn(bool isPersistent)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true).Verifiable();
            IList<string> providers = new List<string>();
            providers.Add("PhoneNumber");
            manager.Setup(m => m.GetValidTwoFactorProvidersAsync(user)).Returns(Task.FromResult(providers)).Verifiable();
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.SupportsUserTwoFactor).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            SetupSignIn(context, auth);
            var id = new ClaimsIdentity(IdentityConstants.TwoFactorRememberMeScheme);
            id.AddClaim(new Claim(ClaimTypes.Name, user.Id));
            auth.Setup(a => a.AuthenticateAsync(context, IdentityConstants.TwoFactorRememberMeScheme))
                .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(id), null, IdentityConstants.TwoFactorRememberMeScheme))).Verifiable();
            var helper = SetupSignInManager(manager.Object, context);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "password", isPersistent, false);

            // Assert
            Assert.True(result.Succeeded);
            manager.Verify();
            auth.Verify();
        }

        private Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
            return auth;
        }

        [Fact]
        public async Task SignOutCallsContextResponseSignOut()
        {
            // Setup
            var manager = MockHelpers.TestUserManager<TestUser>();
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            auth.Setup(a => a.SignOutAsync(context, IdentityConstants.ApplicationScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            auth.Setup(a => a.SignOutAsync(context, IdentityConstants.TwoFactorUserIdScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            auth.Setup(a => a.SignOutAsync(context, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>())).Returns(Task.FromResult(0)).Verifiable();
            var helper = SetupSignInManager(manager, context, null, manager.Options);

            // Act
            await helper.SignOutAsync();

            // Assert
            auth.Verify();
        }

        [Fact]
        public async Task PasswordSignInFailsWithWrongPassword()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "bogus")).ReturnsAsync(false).Verifiable();
            var context = new Mock<HttpContext>();
            var logStore = new StringBuilder();
            var helper = SetupSignInManager(manager.Object, context.Object, logStore);
            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "bogus", false, false);
            var checkResult = await helper.CheckPasswordSignInAsync(user, "bogus", false);

            // Assert
            Assert.False(result.Succeeded);
            Assert.False(checkResult.Succeeded);
            Assert.True(logStore.ToString().Contains($"User {user.Id} failed to provide the correct password."));
            manager.Verify();
            context.Verify();
        }

        [Fact]
        public async Task PasswordSignInFailsWithUnknownUser()
        {
            // Setup
            var manager = MockHelpers.MockUserManager<TestUser>();
            manager.Setup(m => m.FindByNameAsync("bogus")).ReturnsAsync(default(TestUser)).Verifiable();
            var context = new Mock<HttpContext>();
            var helper = SetupSignInManager(manager.Object, context.Object);

            // Act
            var result = await helper.PasswordSignInAsync("bogus", "bogus", false, false);

            // Assert
            Assert.False(result.Succeeded);
            manager.Verify();
            context.Verify();
        }

        [Fact]
        public async Task PasswordSignInFailsWithWrongPasswordCanAccessFailedAndLockout()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            var lockedout = false;
            manager.Setup(m => m.AccessFailedAsync(user)).Returns(() =>
            {
                lockedout = true;
                return Task.FromResult(IdentityResult.Success);
            }).Verifiable();
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).Returns(() => Task.FromResult(lockedout));
            manager.Setup(m => m.CheckPasswordAsync(user, "bogus")).ReturnsAsync(false).Verifiable();
            var context = new Mock<HttpContext>();
            var helper = SetupSignInManager(manager.Object, context.Object);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "bogus", false, true);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsLockedOut);
            manager.Verify();
        }

        [Fact]
        public async Task CheckPasswordSignInFailsWithWrongPasswordCanAccessFailedAndLockout()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            var lockedout = false;
            manager.Setup(m => m.AccessFailedAsync(user)).Returns(() =>
            {
                lockedout = true;
                return Task.FromResult(IdentityResult.Success);
            }).Verifiable();
            manager.Setup(m => m.SupportsUserLockout).Returns(true).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user)).Returns(() => Task.FromResult(lockedout));
            manager.Setup(m => m.CheckPasswordAsync(user, "bogus")).ReturnsAsync(false).Verifiable();
            var context = new Mock<HttpContext>();
            var helper = SetupSignInManager(manager.Object, context.Object);

            // Act
            var result = await helper.CheckPasswordSignInAsync(user, "bogus", true);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsLockedOut);
            manager.Verify();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanRequireConfirmedEmailForPasswordSignIn(bool confirmed)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.IsEmailConfirmedAsync(user)).ReturnsAsync(confirmed).Verifiable();
            if (confirmed)
            {
                manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
            }
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            if (confirmed)
            {
                manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
                SetupSignIn(context, auth);
            }
            var identityOptions = new IdentityOptions();
            identityOptions.SignIn.RequireConfirmedEmail = true;
            var logStore = new StringBuilder();
            var helper = SetupSignInManager(manager.Object, context, logStore, identityOptions);

            // Act
            var result = await helper.PasswordSignInAsync(user, "password", false, false);

            // Assert

            Assert.Equal(confirmed, result.Succeeded);
            Assert.NotEqual(confirmed, result.IsNotAllowed);
            Assert.Equal(confirmed, !logStore.ToString().Contains($"User {user.Id} cannot sign in without a confirmed email."));

            manager.Verify();
            auth.Verify();
        }

        private static void SetupSignIn(HttpContext context, Mock<IAuthenticationService> auth, string userId = null, bool? isPersistent = null, string loginProvider = null)
        {
            auth.Setup(a => a.SignInAsync(context,
                IdentityConstants.ApplicationScheme,
                It.Is<ClaimsPrincipal>(id =>
                    (userId == null || id.FindFirstValue(ClaimTypes.NameIdentifier) == userId) &&
                    (loginProvider == null || id.FindFirstValue(ClaimTypes.AuthenticationMethod) == loginProvider)),
                It.Is<AuthenticationProperties>(v => isPersistent == null || v.IsPersistent == isPersistent))).Returns(Task.FromResult(0)).Verifiable();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanRequireConfirmedPhoneNumberForPasswordSignIn(bool confirmed)
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = SetupUserManager(user);
            manager.Setup(m => m.IsPhoneNumberConfirmedAsync(user)).ReturnsAsync(confirmed).Verifiable();
            var context = new DefaultHttpContext();
            var auth = MockAuth(context);
            if (confirmed)
            {
                manager.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true).Verifiable();
                SetupSignIn(context, auth);
            }

            var identityOptions = new IdentityOptions();
            identityOptions.SignIn.RequireConfirmedPhoneNumber = true;
            var logStore = new StringBuilder();
            var helper = SetupSignInManager(manager.Object, context, logStore, identityOptions);

            // Act
            var result = await helper.PasswordSignInAsync(user, "password", false, false);

            // Assert
            Assert.Equal(confirmed, result.Succeeded);
            Assert.NotEqual(confirmed, result.IsNotAllowed);
            Assert.Equal(confirmed, !logStore.ToString().Contains($"User {user.Id} cannot sign in without a confirmed phone number."));
            manager.Verify();
            auth.Verify();
        }
    }
}
