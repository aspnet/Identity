using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;
using Microsoft.AspNet.Identity.Test;

namespace Microsoft.AspNet.Identity.InMemory.Test
{
    public class InMemoryStoreTest : UserManagerTestBase<IdentityUser, IdentityRole>
    {
        protected override UserManager<IdentityUser> CreateManager()
        {
            var services = new ServiceCollection();
            services.AddTransient<IUserValidator<IdentityUser>, UserValidator<IdentityUser>>();
            services.AddTransient<IPasswordValidator, PasswordValidator>();
            var options = new IdentityOptions
            {
                PasswordsRequireDigit = false,
                PasswordsRequireLowercase = false,
                PasswordsRequireNonLetterOrDigit = false,
                PasswordsRequireUppercase = false
            };
            var optionsAccessor = new OptionsAccessor<IdentityOptions>(new[] {new TestSetup(options)});
            //services.AddInstance<IOptionsAccessor<IdentityOptions>>(optionsAccessor);
            //services.AddInstance<IUserStore<IdentityUser>>(new InMemoryUserStore<IdentityUser>());
            //services.AddSingleton<UserManager<IdentityUser>, UserManager<IdentityUser>>();
            //return services.BuildServiceProvider().GetService<UserManager<IdentityUser>>();
            return new UserManager<IdentityUser>(services.BuildServiceProvider(), new InMemoryUserStore<IdentityUser>(), optionsAccessor);
        }

        protected override RoleManager<IdentityRole> CreateRoleManager()
        {
            var services = new ServiceCollection();
            services.AddTransient<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
            services.AddInstance<IRoleStore<IdentityRole>>(new InMemoryRoleStore<IdentityRole>());
            //return services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();
            return new RoleManager<IdentityRole>(services.BuildServiceProvider(), new InMemoryRoleStore<IdentityRole>());
        }
    }
}