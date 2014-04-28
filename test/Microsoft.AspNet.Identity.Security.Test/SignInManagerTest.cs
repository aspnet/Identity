using System;
using System.Threading;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.Abstractions.Security;
using Microsoft.AspNet.Identity.Test;
using Microsoft.AspNet.DependencyInjection;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Security.Test
{
    public class SignInManagerTest
    {

#if NET45
        [Fact]
        public void ConstructorNullChecks()
        {
            Assert.Throws<ArgumentNullException>("userManager", () => new SignInManager<IdentityUser>(null, null));
            var userManager = MockHelpers.MockUserManager<IdentityUser>().Object;
            Assert.Throws<ArgumentNullException>("contextAccessor", () => new SignInManager<IdentityUser>(userManager, null));
        }

        //TODO: Mock fails in K (this works fine in net45)
        [Fact]
        public async Task EnsureClaimsIdentityFactoryCreateIdentityCalled()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var userManager = MockHelpers.TestUserManager<TestUser>();
            var identityFactory = new Mock<IClaimsIdentityFactory<TestUser>>();
            const string authType = "Test";
            var testIdentity = new ClaimsIdentity(authType);
            identityFactory.Setup(s => s.CreateAsync(userManager, user, authType, CancellationToken.None)).ReturnsAsync(testIdentity).Verifiable();
            userManager.ClaimsIdentityFactory = identityFactory.Object;
            var context = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            context.Setup(c => c.Response).Returns(response.Object).Verifiable();
            response.Setup(r => r.SignIn(testIdentity, It.IsAny<AuthenticationProperties>())).Verifiable();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(userManager, contextAccessor.Object)
            {
                AuthenticationType = authType
            };

            // Act
            await helper.SignInAsync(user, false, false);

            // Assert
            identityFactory.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInReturnsLockedOutWhenLockedOut()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = MockHelpers.MockUserManager<TestUser>();
            manager.Setup(m => m.IsLockedOutAsync(user, CancellationToken.None)).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.FindByNameAsync(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "bogus", false, false);

            // Assert
            Assert.Equal(SignInStatus.LockedOut, result);
            manager.VerifyAll();
        }

        [Fact]
        public async Task CanPasswordSignIn()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = MockHelpers.MockUserManager<TestUser>();
            manager.Setup(m => m.IsLockedOutAsync(user, CancellationToken.None)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.FindByNameAsync(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "password", CancellationToken.None)).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie, CancellationToken.None)).ReturnsAsync(new ClaimsIdentity("Microsoft.AspNet.Identity")).Verifiable();
            var context = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            context.Setup(c => c.Response).Returns(response.Object).Verifiable();
            response.Setup(r => r.SignIn(It.IsAny<ClaimsIdentity>(), It.IsAny<AuthenticationProperties>())).Verifiable();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "password", false, false);

            // Assert
            Assert.Equal(SignInStatus.Success, result);
            manager.VerifyAll();
        }

        [Theory]
        [InlineData("Microsoft.AspNet.Identity.Security.Application")]
        [InlineData("Foo")]
        public void SignOutCallsContextResponseSignOut(string authenticationType)
        {
            // Setup
            var manager = MockHelpers.MockUserManager<TestUser>();
            var context = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            context.Setup(c => c.Response).Returns(response.Object).Verifiable();
            response.Setup(r => r.SignOut(authenticationType)).Verifiable();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object)
            {
                AuthenticationType = authenticationType
            };

            // Act
            helper.SignOut();

            // Assert
            context.VerifyAll();
            response.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInFailsWithWrongPassword()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = MockHelpers.MockUserManager<TestUser>();
            manager.Setup(m => m.IsLockedOutAsync(user, CancellationToken.None)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.FindByNameAsync(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "bogus", CancellationToken.None)).ReturnsAsync(false).Verifiable();
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object);
            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "bogus", false, false);

            // Assert
            Assert.Equal(SignInStatus.Failure, result);
            manager.VerifyAll();
        }


        [Fact]
        public async Task PasswordSignInFailsWithUnknownUser()
        {
            // Setup
            var manager = MockHelpers.MockUserManager<TestUser>();
            manager.Setup(m => m.FindByNameAsync("bogus", CancellationToken.None)).ReturnsAsync(null).Verifiable();
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object);

            // Act
            var result = await helper.PasswordSignInAsync("bogus", "bogus", false, false);

            // Assert
            Assert.Equal(SignInStatus.Failure, result);
            manager.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInFailsWithWrongPasswordCanAccessFailedAndLockout()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = MockHelpers.MockUserManager<TestUser>();
            var lockedout = false;
            manager.Setup(m => m.AccessFailedAsync(user, CancellationToken.None)).Returns(() =>
            {
                lockedout = true;
                return Task.FromResult(IdentityResult.Success);
            }).Verifiable();
            manager.Setup(m => m.IsLockedOutAsync(user, CancellationToken.None)).Returns(() => Task.FromResult(lockedout));
            manager.Setup(m => m.FindByNameAsync(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            manager.Setup(m => m.CheckPasswordAsync(user, "bogus", CancellationToken.None)).ReturnsAsync(false).Verifiable();
            var context = new Mock<HttpContext>();
            var contextAccessor = new Mock<IContextAccessor<HttpContext>>();
            contextAccessor.Setup(a => a.Value).Returns(context.Object);
            var helper = new SignInManager<TestUser>(manager.Object, contextAccessor.Object);

            // Act
            var result = await helper.PasswordSignInAsync(user.UserName, "bogus", false, true);

            // Assert
            Assert.Equal(SignInStatus.LockedOut, result);
            manager.VerifyAll();
        }
#endif
    }
}