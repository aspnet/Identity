using System.Security.Claims;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class IdentityBuilderTest
    {
        [Fact]
        public void CanSpecifyUserValidatorInstance()
        {
            var services = new ServiceCollection();
            var validator = new UserValidator<IdentityUser>(new IdentityOptions());
            services.AddIdentity<IdentityUser>(b => b.UseUserValidator(() => validator));
            Assert.Equal(validator, services.BuildServiceProvider().GetService<IUserValidator<IdentityUser>>());
        }

        [Fact]
        public void CanSpecifyPasswordValidatorInstance()
        {
            var services = new ServiceCollection();
            var validator = new PasswordValidator(new IdentityOptions());
            services.AddIdentity<IdentityUser>(b => b.UsePasswordValidator(() => validator));
            Assert.Equal(validator, services.BuildServiceProvider().GetService<IPasswordValidator>());
        }

        [Fact]
        public void CanSpecifyPasswordHasherInstance()
        {
            CanOverride<IPasswordHasher>(new PasswordHasher());
        }

        [Fact]
        public void CanSpecifyClaimsIdentityFactoryInstance()
        {
            CanOverride<IClaimsIdentityFactory<IdentityUser>>(new ClaimsIdentityFactory<IdentityUser>(new IdentityOptions()));
        }

        [Fact]
        public void EnsureDefaultServices()
        {
            var services = new ServiceCollection();
            services.AddIdentity<IdentityUser>(identity => { });

            var provider = services.BuildServiceProvider();
            var userValidator = provider.GetService<IUserValidator<IdentityUser>>() as UserValidator<IdentityUser>;
            Assert.NotNull(userValidator);

            var pwdValidator = provider.GetService<IPasswordValidator>() as PasswordValidator;
            Assert.NotNull(pwdValidator);

            var hasher = provider.GetService<IPasswordHasher>() as PasswordHasher;
            Assert.NotNull(hasher);

            var claimsFactory = provider.GetService<IClaimsIdentityFactory<IdentityUser>>() as ClaimsIdentityFactory<IdentityUser>;
            Assert.NotNull(claimsFactory);
        }

        private static void CanOverride<TService>(TService instance)
        {
            var services = new ServiceCollection();
            services.AddIdentity<IdentityUser>(b => b.Use<TService>(() => instance));
            Assert.Equal(instance, services.BuildServiceProvider().GetService<TService>());
        }

    }
}