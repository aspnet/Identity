using System;
using System.Collections.Generic;
using Microsoft.AspNet.ConfigurationModel;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    /// Default services used by UserManagers
    /// </summary>
    public class UserManagerServices<TUser> where TUser : class
    {

        public static IEnumerable<IServiceDescriptor> GetDefaultServices()
        {
            return GetDefaultServices(new Configuration());
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration)
        {
            var describe = new ServiceDescriber(configuration);

            // TODO: review defaults for validators should get picked up from config?
            yield return describe.Instance<IUserValidator<TUser>>(new UserValidator<TUser>());
            yield return describe.Instance<IPasswordValidator>(new PasswordValidator());
            yield return describe.Instance<IPasswordHasher>(new PasswordHasher());
            yield return describe.Instance<IClaimsIdentityFactory<TUser>>(new ClaimsIdentityFactory<TUser>());
            yield return describe.Instance<LockoutPolicy>(new LockoutPolicy
            {
                UserLockoutEnabledByDefault = false,
                DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5),
                MaxFailedAccessAttemptsBeforeLockout = 5
            });

            // TODO: rationalize email/sms/usertoken services
            // TODO: configure lockout from config?
        }
    }


    public class UserManagerBuilder<TUser> where TUser : class 
    {
        private ServiceCollection Services { get; set; }

        public UserManagerBuilder(ServiceCollection services)
        {
            Services = services;
        }

        public UserManagerBuilder<TUser> Use<T>(Func<T> func)
        {
            Services.AddInstance<T>(func());
            return this;
        }

        public UserManagerBuilder<TUser> UseIdentity()
        {
            Services.Add(UserManagerServices<TUser>.GetDefaultServices());
            return this;
        }

        public UserManagerBuilder<TUser> UseStore(Func<IUserStore<TUser>> func)
        {
            return Use(func);
        }

        public UserManagerBuilder<TUser> UsePasswordValidator(Func<IPasswordValidator> func)
        {
            return Use(func);
        }

        public UserManagerBuilder<TUser> UseUserValidator(Func<IUserValidator<TUser>> func)
        {
            return Use(func);
        }

        public UserManagerBuilder<TUser> UseManager<TManager>() where TManager : UserManager<TUser>
        {
            Services.AddSingleton<TManager, TManager>();
            return this;
        }

        //public UserManagerBuilder<TUser> UseTwoFactorProviders(Func<IDictionary<string, IUserTokenProvider<TUser>>> func)
        //{
        //    return Use(func);
        //}

        public UserManagerBuilder<TUser> UseLockoutPolicy(Func<LockoutPolicy> func)
        {
            return Use(func);
        }

    }

    public static class ServiceCollectionExtensions
    {
        public static UserManagerBuilder<TUser> AddIdentity<TUser>(this ServiceCollection services, Action<UserManagerBuilder<TUser>> actionBuilder) where TUser : class
        {
            services.Add(UserManagerServices<TUser>.GetDefaultServices());
            var builder = new UserManagerBuilder<TUser>(services);
            actionBuilder(builder);
            return builder;
        }
    }
}