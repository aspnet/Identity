using System.Threading;
using Microsoft.AspNet.HttpFeature.Security;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Security.Test
{
    public class SignInManagerTest
    {

#if NET45
        //TODO: Mock fails in K (this works fine in net45)
        [Fact]
        public async Task EnsureClaimsIdentityFactoryCreateIdentityCalled()
        {
            // Setup
            var store = new Mock<IUserStore<TestUser, string>>();
            var user = new TestUser { UserName = "Foo" };
            var userManager = new UserManager<TestUser, string>(store.Object);
            var identityFactory = new Mock<IClaimsIdentityFactory<TestUser, string>>();
            var testIdentity = new ClaimsIdentity("Microsoft.AspNet.Identity");
            identityFactory.Setup(s => s.Create(userManager, user, "Microsoft.AspNet.Identity", CancellationToken.None)).ReturnsAsync(testIdentity).Verifiable();
            userManager.ClaimsIdentityFactory = identityFactory.Object;
            var helper = new SignInManager<TestUser, string> { UserManager = userManager };

            // Act
            var result = await helper.SignIn(user, false, false);

            // Assert
            Assert.IsAssignableFrom(typeof(IAuthenticationSignIn), result);
            identityFactory.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInReturnsLockedOutWhenLockedOut()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = new Mock<UserManager<TestUser, string>>();
            manager.Setup(m => m.IsLockedOut(user.Id, CancellationToken.None)).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.FindByName(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            var helper = new SignInManager<TestUser, string> { UserManager = manager.Object };

            // Act
            var result = await helper.PasswordSignIn(user.UserName, "bogus", false, false);

            // Assert
            Assert.Equal(SignInStatus.LockedOut, result);
            manager.VerifyAll();
        }

        [Fact]
        public async Task CanPasswordSignIn()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = new Mock<UserManager<TestUser, string>>();
            manager.Setup(m => m.IsLockedOut(user.Id, CancellationToken.None)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.FindByName(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            manager.Setup(m => m.CheckPassword(user, "password", CancellationToken.None)).ReturnsAsync(true).Verifiable();
            manager.Setup(m => m.CreateIdentity(user, "Microsoft.AspNet.Identity", CancellationToken.None)).ReturnsAsync(new ClaimsIdentity("Microsoft.AspNet.Identity")).Verifiable();
            var helper = new SignInManager<TestUser, string> { UserManager = manager.Object };

            // Act
            var result = await helper.PasswordSignIn(user.UserName, "password", false, false);

            // Assert
            Assert.Equal(SignInStatus.Success, result);
            manager.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInFailsWithWrongPassword()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = new Mock<UserManager<TestUser, string>>();
            manager.Setup(m => m.IsLockedOut(user.Id, CancellationToken.None)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.FindByName(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            manager.Setup(m => m.CheckPassword(user, "bogus", CancellationToken.None)).ReturnsAsync(false).Verifiable();
            var helper = new SignInManager<TestUser, string> { UserManager = manager.Object };

            // Act
            var result = await helper.PasswordSignIn(user.UserName, "bogus", false, false);

            // Assert
            Assert.Equal(SignInStatus.Failure, result);
            manager.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInFailsWithUnknownUser()
        {
            // Setup
            var manager = new Mock<UserManager<TestUser, string>>();
            manager.Setup(m => m.FindByName("bogus", CancellationToken.None)).ReturnsAsync(null).Verifiable();
            var helper = new SignInManager<TestUser, string> { UserManager = manager.Object };

            // Act
            var result = await helper.PasswordSignIn("bogus", "bogus", false, false);

            // Assert
            Assert.Equal(SignInStatus.Failure, result);
            manager.VerifyAll();
        }

        [Fact]
        public async Task PasswordSignInFailsWithWrongPasswordShouldLockoutCallsAccessFailed()
        {
            // Setup
            var user = new TestUser { UserName = "Foo" };
            var manager = new Mock<UserManager<TestUser, string>>();
            manager.Setup(m => m.IsLockedOut(user.Id, CancellationToken.None)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.FindByName(user.UserName, CancellationToken.None)).ReturnsAsync(user).Verifiable();
            manager.Setup(m => m.CheckPassword(user, "bogus", CancellationToken.None)).ReturnsAsync(false).Verifiable();
            manager.Setup(m => m.AccessFailed(user.Id, CancellationToken.None)).ReturnsAsync(IdentityResult.Success).Verifiable();
            var helper = new SignInManager<TestUser, string> { UserManager = manager.Object };

            // Act
            var result = await helper.PasswordSignIn(user.UserName, "bogus", false, true);

            // Assert
            Assert.Equal(SignInStatus.Failure, result);
            manager.VerifyAll();
        }


#endif
    }
}