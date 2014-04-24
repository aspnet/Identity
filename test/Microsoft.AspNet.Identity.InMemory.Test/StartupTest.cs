using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.AspNet.PipelineCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.InMemory.Test
{
    public class StartupTest
    {
        [Fact]
        public void EnsureSetupUsageWorksOnMyInstance()
        {
            IBuilder builder = new Builder(new ServiceCollection().BuildServiceProvider());
            var myOptions = new IdentityOptions();
            builder.UseServices(services => {
                services.AddIdentity<IdentityUser>(identityServices => { });
                services.AddInstance<IdentityOptions>(myOptions);
            });

            var setup = builder.ApplicationServices.GetService<IOptionsSetup<IdentityOptions>>();
            Assert.IsType(typeof(DefaultIdentitySetup), setup);
            var optionsGetter = builder.ApplicationServices.GetService<IOptionsAccessor<IdentityOptions>>();
            Assert.NotNull(optionsGetter);
            setup.Setup(optionsGetter.Options);

            Assert.Equal(myOptions, optionsGetter.Options);

            Assert.True(myOptions.PasswordsRequireLowercase);
            Assert.True(myOptions.PasswordsRequireDigit);
            Assert.True(myOptions.PasswordsRequireNonLetterOrDigit);
            Assert.True(myOptions.PasswordsRequireUppercase);
            Assert.Equal(6, myOptions.PasswordsRequiredLength);
        }

        public class NoopIdentitySetup : IOptionsSetup<IdentityOptions>
        {
            public int ExecutionOrder { get; set; }
            public void Setup(IdentityOptions options)
            {
            }
        }

        [Fact]
        public void CanOverrideDisableSetup()
        {
            IBuilder builder = new Builder(new ServiceCollection().BuildServiceProvider());
            var myOptions = new IdentityOptions();
            builder.UseServices(services =>
            {
                services.AddIdentity<IdentityUser>(identityServices => { });
                services.AddInstance<IdentityOptions>(myOptions);
                services.AddSetup<NoopIdentitySetup>();
            });

            var setup = builder.ApplicationServices.GetService<IOptionsSetup<IdentityOptions>>();
            Assert.IsType(typeof(NoopIdentitySetup), setup);

            var optionsGetter = builder.ApplicationServices.GetService<IOptionsAccessor<IdentityOptions>>();
            Assert.NotNull(optionsGetter);
            setup.Setup(optionsGetter.Options);

            Assert.Equal(myOptions, optionsGetter.Options);

            Assert.False(myOptions.PasswordsRequireLowercase);
            Assert.False(myOptions.PasswordsRequireDigit);
            Assert.False(myOptions.PasswordsRequireNonLetterOrDigit);
            Assert.False(myOptions.PasswordsRequireUppercase);
            Assert.Equal(0, myOptions.PasswordsRequiredLength);
        }

        [Fact]
        public async Task EnsureStartupUsageWorks()
        {
            IBuilder builder = new Builder(new ServiceCollection().BuildServiceProvider());

            builder.UseServices(services => services.AddIdentity<ApplicationUser>(s =>
            {
                s.UseUserStore(() => new InMemoryUserStore<ApplicationUser>());
                s.UseUserManager<ApplicationUserManager>();
                s.UseRoleStore(() => new InMemoryRoleStore<IdentityRole>());
                s.UseRoleManager<ApplicationRoleManager>();
            }));

            var userStore = builder.ApplicationServices.GetService<IUserStore<ApplicationUser>>();
            var roleStore = builder.ApplicationServices.GetService<IRoleStore<IdentityRole>>();
            var userManager = builder.ApplicationServices.GetService<ApplicationUserManager>();
            var roleManager = builder.ApplicationServices.GetService<ApplicationRoleManager>();

            Assert.NotNull(userStore);
            Assert.NotNull(userManager);
            Assert.NotNull(roleStore);
            Assert.NotNull(roleManager);

            await CreateAdminUser(builder.ApplicationServices);
        }

        private static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            const string userName = "admin";
            const string roleName = "Admins";
            const string password = "1qaz@WSX";
            var userManager = serviceProvider.GetService<ApplicationUserManager>();
            var roleManager = serviceProvider.GetService<ApplicationRoleManager>();

            var user = new ApplicationUser { UserName = userName };
            IdentityResultAssert.IsSuccess(await userManager.CreateAsync(user, password));
            IdentityResultAssert.IsSuccess(await roleManager.CreateAsync(new IdentityRole { Name = roleName }));
            IdentityResultAssert.IsSuccess(await userManager.AddToRoleAsync(user, roleName));
        }


        public class ApplicationUserManager : UserManager<ApplicationUser>
        {
            public ApplicationUserManager(IServiceProvider services, IUserStore<ApplicationUser> store) : base(services, store) { }
        }

        public class ApplicationRoleManager : RoleManager<IdentityRole>
        {
            public ApplicationRoleManager(IServiceProvider services, IRoleStore<IdentityRole> store) : base(services, store) { }
        }

        public class ApplicationUser : IdentityUser
        {
        }

    }
}