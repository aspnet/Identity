using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class SignInHelperTest
    {

#if NET45
        //TODO: Mock fails in K (this works fine in net45)
        [Fact]
        public async Task SignInProviderCalled()
        {
            // Setup
            var store = new Mock<IUserStore<TestUser, string>>();
            var user = new TestUser { UserName = "Foo" };
            var sub = new Mock<INotificationSubscriber>();
            sub.Setup(s => s.Notify(IdentityNotificationTopics.UserManagerCreate, It.Is<UserEvent<TestUser,string>>(e => e.Result.Succeeded && e.User == user))).Returns(Task.FromResult(0)).Verifiable();
            store.Setup(s => s.Create(user)).Returns(Task.FromResult(0)).Verifiable();
            var validator = new Mock<UserValidator<TestUser, string>>();
            var userManager = new UserManager<TestUser, string>(store.Object);
            validator.Setup(v => v.Validate(userManager, user)).ReturnsAsync(IdentityResult.Success).Verifiable();
            userManager.UserValidator = validator.Object;
            var identityProvider = new Mock<IClaimsIdentityProvider<TestUser, string>>();
            var claimsIdentity =
            identityProvider.Setup(s => s.CreateUserIdentity(userManager, user)).ReturnsAsync()
            var helper = new SignInHelper(userManager, claimsIdentityProvider.Object);

            // Act
            var result = await userManager.Create(user);

            // Assert
            Assert.True(result.Succeeded);
            store.VerifyAll();
            sub.VerifyAll();
        }
#endif
    }
}