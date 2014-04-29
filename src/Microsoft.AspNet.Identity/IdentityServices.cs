using Microsoft.AspNet.ConfigurationModel;
using Microsoft.AspNet.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    /// Default services used by UserManager and RoleManager
    /// </summary>
    public class IdentityServices
    {

        public static IEnumerable<IServiceDescriptor> GetDefaultUserServices<TUser>() where TUser : class
        {
            return GetDefaultUserServices<TUser>(new Configuration());
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultUserServices<TUser>(IConfiguration configuration) where TUser : class
        {
            var describe = new ServiceDescriber(configuration);

            // TODO: review defaults for validators should get picked up from config?
            yield return describe.Transient<IUserValidator<TUser>, UserValidator<TUser>>();
            yield return describe.Transient<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            yield return describe.Transient<IPasswordHasher, PasswordHasher>();
            yield return describe.Transient<IClaimsIdentityFactory<TUser>, ClaimsIdentityFactory<TUser>>();

            // TODO: rationalize email/sms/usertoken services
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultRoleServices<TRole>() where TRole : class
        {
            return GetDefaultRoleServices<TRole>(new Configuration());
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultRoleServices<TRole>(IConfiguration configuration) where TRole : class
        {
            var describe = new ServiceDescriber(configuration);

            // TODO: review defaults for validators should get picked up from config?
            yield return describe.Instance<IRoleValidator<TRole>>(new RoleValidator<TRole>());
        }
    }
}