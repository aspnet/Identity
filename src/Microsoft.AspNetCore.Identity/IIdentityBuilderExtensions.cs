// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Helper functions for configuring identity services.
    /// </summary>
    public static class IIdentityBuilderExtensions
    {
        private static IIdentityBuilder AddScoped(IIdentityBuilder builder, Type serviceType, Type concreteType)
        {
            builder.Services.AddScoped(serviceType, concreteType);
            return builder;
        }

        /// <summary>
        /// Adds an <see cref="IUserValidator{TUser}"/> for the <seealso cref="IIdentityBuilder.UserType"/>.
        /// </summary>
        /// <typeparam name="T">The user type to validate.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddUserValidator<T>(this IIdentityBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddScoped(builder, typeof(IUserValidator<>).MakeGenericType(builder.UserType), typeof(T));
        }

        /// <summary>
        /// Adds an <see cref="IRoleValidator{TRole}"/> for the <seealso cref="IIdentityBuilder.RoleType"/>.
        /// </summary>
        /// <typeparam name="T">The role type to validate.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddRoleValidator<T>(this IIdentityBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddScoped(builder, typeof(IRoleValidator<>).MakeGenericType(builder.RoleType), typeof(T));
        }

        /// <summary>
        /// Adds an <see cref="IdentityErrorDescriber"/>.
        /// </summary>
        /// <typeparam name="TDescriber">The type of the error describer.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddErrorDescriber<TDescriber>(this IIdentityBuilder builder) 
            where TDescriber : IdentityErrorDescriber
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddScoped<IdentityErrorDescriber, TDescriber>();
            return builder;
        }

        /// <summary>
        /// Adds an <see cref="IPasswordValidator{TUser}"/> for the <seealso cref="IIdentityBuilder.UserType"/>.
        /// </summary>
        /// <typeparam name="T">The user type whose password will be validated.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddPasswordValidator<T>(this IIdentityBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddScoped(builder, typeof(IPasswordValidator<>).MakeGenericType(builder.UserType), typeof(T));
        }

        /// <summary>
        /// Adds an <see cref="IUserStore{TUser}"/> for the <seealso cref="IIdentityBuilder.UserType"/>.
        /// </summary>
        /// <typeparam name="T">The user type whose password will be validated.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddUserStore<T>(this IIdentityBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddScoped(builder, typeof(IUserStore<>).MakeGenericType(builder.UserType), typeof(T));
        }

        /// <summary>
        /// Adds a <see cref="IRoleStore{TRole}"/> for the <seealso cref="IIdentityBuilder.RoleType"/>.
        /// </summary>
        /// <typeparam name="T">The role type held in the store.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddRoleStore<T>(this IIdentityBuilder builder) where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddScoped(builder, typeof(IRoleStore<>).MakeGenericType(builder.RoleType), typeof(T));
        }

        /// <summary>
        /// Adds a token provider.
        /// </summary>
        /// <typeparam name="TProvider">The type of the token provider to add.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <param name="providerName">The name of the provider to add.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddTokenProvider<TProvider>(this IIdentityBuilder builder, string providerName)
            where TProvider : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddTokenProvider(providerName, typeof(TProvider));
        }

        /// <summary>
        /// Adds a token provider for the <seealso cref="IIdentityBuilder.UserType"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <param name="providerName">The name of the provider to add.</param>
        /// <param name="provider">The type of the <see cref="IUserTokenProvider{TUser}"/> to add.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddTokenProvider(this IIdentityBuilder builder, string providerName, Type provider)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (!typeof(IUserTokenProvider<>).MakeGenericType(builder.UserType).GetTypeInfo().IsAssignableFrom(provider.GetTypeInfo()))
            {
                throw new InvalidOperationException(Resources.FormatInvalidManagerType(provider.Name, "IUserTokenProvider", builder.UserType.Name));
            }

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.ProviderMap[providerName] = new TokenProviderDescriptor(provider);
            });
            builder.Services.AddTransient(provider);
            return builder;
        }

        /// <summary>
        /// Adds the default token providers used to generate tokens for reset passwords, change email
        /// and change telephone number operations, and for two factor authentication token generation.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddDefaultTokenProviders(this IIdentityBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var dataProtectionProviderType = typeof(DataProtectorTokenProvider<>).MakeGenericType(builder.UserType);
            var phoneNumberProviderType = typeof(PhoneNumberTokenProvider<>).MakeGenericType(builder.UserType);
            var emailTokenProviderType = typeof(EmailTokenProvider<>).MakeGenericType(builder.UserType);
            return builder.AddTokenProvider(TokenOptions.DefaultProvider, dataProtectionProviderType)
                .AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType)
                .AddTokenProvider(TokenOptions.DefaultPhoneProvider, phoneNumberProviderType);
        }

        /// <summary>
        /// Adds a <see cref="UserManager{TUser}"/> for the <seealso cref="IIdentityBuilder.UserType"/>.
        /// </summary>
        /// <typeparam name="TUserManager">The type of the user manager to add.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddUserManager<TUserManager>(this IIdentityBuilder builder) where TUserManager : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var userManagerType = typeof(UserManager<>).MakeGenericType(builder.UserType);
            var customType = typeof(TUserManager);
            if (userManagerType == customType ||
                !userManagerType.GetTypeInfo().IsAssignableFrom(customType.GetTypeInfo()))
            {
                throw new InvalidOperationException(Resources.FormatInvalidManagerType(customType.Name, "UserManager", builder.UserType.Name));
            }
            builder.Services.AddScoped(customType, services => services.GetRequiredService(userManagerType));
            return AddScoped(builder, userManagerType, customType);
        }

        /// <summary>
        /// Adds a <see cref="RoleManager{TRole}"/> for the <seealso cref="IIdentityBuilder.RoleType"/>.
        /// </summary>
        /// <typeparam name="TRoleManager">The type of the role manager to add.</typeparam>
        /// <param name="builder">The <see cref="IIdentityBuilder"/>.</param>
        /// <returns>The current <see cref="IIdentityBuilder"/> instance.</returns>
        public static IIdentityBuilder AddRoleManager<TRoleManager>(this IIdentityBuilder builder) where TRoleManager : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var managerType = typeof(RoleManager<>).MakeGenericType(builder.RoleType);
            var customType = typeof(TRoleManager);
            if (managerType == customType ||
                !managerType.GetTypeInfo().IsAssignableFrom(customType.GetTypeInfo()))
            {
                throw new InvalidOperationException(Resources.FormatInvalidManagerType(customType.Name, "RoleManager", builder.RoleType.Name));
            }
            builder.Services.AddScoped(typeof(TRoleManager), services => services.GetRequiredService(managerType));
            return AddScoped(builder, managerType, typeof(TRoleManager));
        }
    }
}
