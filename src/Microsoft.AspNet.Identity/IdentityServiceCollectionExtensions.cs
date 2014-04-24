using Microsoft.AspNet.Identity;
using System;
using System.Linq;

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
            services.AddSingleton<IdentityOptions, IdentityOptions>(); // TODO: what is the correct lifetime?
            services.AddTransient<IOptionsAccessor<IdentityOptions>, IdentityOptionsAccessor>();
            services.AddSetup<DefaultIdentitySetup>(); // TODO: add overload which doesn't take setup?
            actionBuilder(new IdentityBuilder<TUser, TRole>(services));
            return services;
        }

        public static ServiceCollection AddIdentity<TUser>(this ServiceCollection services, Action<IdentityBuilder<TUser, IdentityRole>> actionBuilder)
            where TUser : class
        {
            return services.AddIdentity<TUser, IdentityRole>(actionBuilder);
        }

        // TODO: find a non identity home for this (DI?)
        public static ServiceCollection AddSetup(this ServiceCollection services, Type type)
        {
#if NET45
            // No GetInterfaces() in K
            var setupTypes = type.GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IOptionsSetup<>));
            foreach (var setupType in setupTypes)
            {
                services.Add(new ServiceDescriptor
                {
                    ServiceType = setupType,
                    ImplementationType = type,
                    Lifecycle = LifecycleKind.Transient
                });
            }
#endif
            return services;
        }

        // TODO: find a non identity home for this (DI?)
        public static ServiceCollection AddSetup(this ServiceCollection services, object setupInstance)
        {
            return services.AddSetup(setupInstance.GetType());
        }

        // TODO: setups?
        // TODO: find a non identity home for this (DI?)
        public static ServiceCollection AddSetup<TSetup>(this ServiceCollection services) where TSetup : class
        {
            return services.AddSetup(typeof(TSetup));
        }

    }
}