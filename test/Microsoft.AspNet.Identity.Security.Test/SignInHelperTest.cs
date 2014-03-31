using Microsoft.AspNet.HttpFeature.Security;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Security.Test
{
    public class SignInHelperTest
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
            identityFactory.Setup(s => s.Create(userManager, user, "Microsoft.AspNet.Identity")).ReturnsAsync(testIdentity).Verifiable();
            userManager.ClaimsIdentityFactory = identityFactory.Object;
            var helper = new SignInManager<TestUser, string> { UserManager = userManager };

            // Act
            var result = await helper.SignIn(user, false, false);

            // Assert
            Assert.IsAssignableFrom(typeof(IAuthenticationSignIn), result);
            identityFactory.VerifyAll();
        }

#endif
    }
}