using Microsoft.AspNet.Identity;
using System;

namespace Microsoft.AspNet.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static ServiceCollection AddIdentity<TUser, TRole>(this ServiceCollection services, Action<IdentityBuilder<TUser, TRole>> actionBuilder)
            where TUser : class
            where TRole : class
        {
            services.Add(IdentityServices.GetDefaultUserServices<TUser>());
            services.Add(IdentityServices.GetDefaultRoleServices<TRole>());
            services.AddSingleton<IOptionsAccessor<IdentityOptions>, OptionsAccessor<IdentityOptions>>();
            services.AddSetup<DefaultIdentitySetup>(); // TODO: add overload which doesn't take setup?
            actionBuilder(new IdentityBuilder<TUser, TRole>(services));
            return services;
        }

        public static ServiceCollection AddIdentity<TUser>(this ServiceCollection services, Action<IdentityBuilder<TUser, IdentityRole>> actionBuilder)
            where TUser : class
        {
            return services.AddIdentity<TUser, IdentityRole>(actionBuilder);
        }
    }
}