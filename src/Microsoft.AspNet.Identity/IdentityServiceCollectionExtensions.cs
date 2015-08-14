// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection.Extensions;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/> for configuring identity services.
    /// </summary>
    public static class IdentityServiceCollectionExtensions
    {
        /// <summary>
        /// Configures a set of <see cref="IdentityOptions"/> for the application
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance this method extends.</returns>
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction)
        {
            return services.Configure(setupAction);
        }

        /// <summary>
        /// Configures a set of <see cref="IdentityOptions"/> for the application
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="config">The configuration for the <see cref="IdentityOptions>"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance this method extends.</returns>
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration config)
        {
            return services.Configure<IdentityOptions>(config);
        }

        /// <summary>
        /// Configures a set of <see cref="CookieAuthenticationOptions"/> for the application
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="CookieAuthenticationOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance this method extends.</returns>
        public static IServiceCollection ConfigureIdentityApplicationCookie(this IServiceCollection services, Action<CookieAuthenticationOptions> setupAction)
        {
            return services.ConfigureCookieAuthentication(setupAction, IdentityOptions.ApplicationCookieAuthenticationScheme);
        }

        /// <summary>
        /// Adds the default identity system configuration for the specified User and Role types.
        /// </summary>
        /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
        /// <typeparam name="TRole">The type representing a Role in the system.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddIdentity<TUser, TRole>(
            this IServiceCollection services)
            where TUser : class
            where TRole : class
        {
            return services.AddIdentity<TUser, TRole>(setupAction: null);
        }

        /// <summary>
        /// Adds and configures the identity system for the specified User and Role types.
        /// </summary>
        /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
        /// <typeparam name="TRole">The type representing a Role in the system.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddIdentity<TUser, TRole>(
            this IServiceCollection services, 
            Action<IdentityOptions> setupAction)
            where TUser : class
            where TRole : class
        {
            // Services used by identity
            services.AddOptions();
            services.AddAuthentication();

            // Identity services
            services.TryAdd(ServiceDescriptor.Scoped<IUserValidator, UserValidator>());
            services.TryAdd(ServiceDescriptor.Scoped<IPasswordValidator, PasswordValidator>());
            services.TryAdd(ServiceDescriptor.Scoped<IPasswordHasher, PasswordHasher>());
            services.TryAdd(ServiceDescriptor.Scoped<ILookupNormalizer, UpperInvariantLookupNormalizer>());
            services.TryAdd(ServiceDescriptor.Scoped<IRoleValidator, RoleValidator>());
            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAdd(ServiceDescriptor.Scoped<IdentityErrorDescriber, IdentityErrorDescriber>());
            services.TryAdd(ServiceDescriptor.Scoped<ISecurityStampValidator, SecurityStampValidator<TUser>>());
            services.TryAdd(ServiceDescriptor.Scoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>());
            services.TryAdd(ServiceDescriptor.Scoped<UserManager<TUser>, UserManager<TUser>>());
            services.TryAdd(ServiceDescriptor.Scoped<SignInManager<TUser>, SignInManager<TUser>>());
            services.TryAdd(ServiceDescriptor.Scoped<RoleManager<TRole>, RoleManager<TRole>>());

            if (setupAction != null)
            {
                services.ConfigureIdentity(setupAction);
            }
            services.Configure<SharedAuthenticationOptions>(options =>
            {
                options.SignInScheme = IdentityOptions.ExternalCookieAuthenticationScheme;
            });

            // Configure all of the cookie middlewares
            services.ConfigureIdentityApplicationCookie(options =>
            {
                options.AuthenticationScheme = IdentityOptions.ApplicationCookieAuthenticationScheme;
                options.AutomaticAuthentication = true;
                options.LoginPath = new PathString("/Account/Login");
                options.Notifications = new CookieAuthenticationNotifications
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
            });

            services.ConfigureCookieAuthentication(options =>
            {
                options.AuthenticationScheme = IdentityOptions.ExternalCookieAuthenticationScheme;
                options.CookieName = IdentityOptions.ExternalCookieAuthenticationScheme;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            }, IdentityOptions.ExternalCookieAuthenticationScheme);
            services.ConfigureCookieAuthentication(options =>
            {
                options.AuthenticationScheme = IdentityOptions.TwoFactorRememberMeCookieAuthenticationScheme;
                options.CookieName = IdentityOptions.TwoFactorRememberMeCookieAuthenticationScheme;
            }, IdentityOptions.TwoFactorRememberMeCookieAuthenticationScheme);
            services.ConfigureCookieAuthentication(options =>
            {
                options.AuthenticationScheme = IdentityOptions.TwoFactorUserIdCookieAuthenticationScheme;
                options.CookieName = IdentityOptions.TwoFactorUserIdCookieAuthenticationScheme;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            }, IdentityOptions.TwoFactorUserIdCookieAuthenticationScheme);

            return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
        }
    }
}