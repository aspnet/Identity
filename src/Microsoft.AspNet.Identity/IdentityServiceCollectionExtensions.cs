using Microsoft.AspNet.Identity;
using System;

namespace Microsoft.AspNet.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IdentityBuilder<TUser, TRole> AddIdentity<TUser, TRole>(this ServiceCollection services, Action<IdentityBuilder<TUser, TRole>> actionBuilder)
            where TUser : class
            where TRole : class
        {
            services.Add(IdentityServices.GetDefaultUserServices<TUser>());
            services.Add(IdentityServices.GetDefaultRoleServices<TRole>());
            var builder = new IdentityBuilder<TUser, TRole>(services);
            actionBuilder(builder);
            return builder;
        }
    }
}