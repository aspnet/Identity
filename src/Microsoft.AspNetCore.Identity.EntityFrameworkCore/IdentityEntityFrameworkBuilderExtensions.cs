// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class IdentityEntityFrameworkBuilderExtensions
    {
        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores with the schema/features in
        /// <see cref="IdentityStoreOptions.Version1_0"/>.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddEntityFrameworkStoresV1<TContext>(this IdentityBuilder builder)
            where TContext : DbContext
        {
            builder.Services.Configure<IdentityStoreOptions>(o => o.Version = IdentityStoreOptions.Version1_0);
            AddStores(builder.Services, typeof(UserStoreV1<,,,,,,,,>), typeof(RoleStoreV1<,,,,>), builder.UserType, builder.RoleType, typeof(TContext));
            return builder;
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores with the schema/features in
        /// <see cref="IdentityStoreOptions.Version2_0"/>.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddEntityFrameworkStoresV2<TContext>(this IdentityBuilder builder)
            where TContext : DbContext
        {
            builder.Services.Configure<IdentityStoreOptions>(o => o.Version = IdentityStoreOptions.Version2_0);
            // Note: RoleStore was not changed for V2.
            AddStores(builder.Services, typeof(UserStoreV2<,,,,,,,,>), typeof(RoleStoreV1<,,,,>), builder.UserType, builder.RoleType, typeof(TContext));
            return builder;
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores with the latest schema/features in
        /// <see cref="IdentityStoreOptions.Version"/>.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddEntityFrameworkStoresLatest<TContext>(this IdentityBuilder builder)
            where TContext : DbContext
        {
            builder.Services.Configure<IdentityStoreOptions>(o => o.Version = IdentityStoreOptions.Version_Latest);
            AddStores(builder.Services, typeof(UserStore<,,,,,,,,>), typeof(RoleStore<,,,,>), builder.UserType, builder.RoleType, typeof(TContext));
            return builder;
        }

        /// <summary>
        /// Adds an Entity Framework implementation of identity information stores.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddEntityFrameworkStores<TContext>(this IdentityBuilder builder)
            where TContext : DbContext
            => builder.AddEntityFrameworkStoresV1<TContext>();

        private static void AddStores(IServiceCollection services, Type userStore, Type roleStore, Type userType, Type roleType, Type contextType)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<,,,,>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException(Resources.NotIdentityUser);
            }
            var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<,,>));
            if (identityRoleType == null)
            {
                throw new InvalidOperationException(Resources.NotIdentityRole);
            }

            services.TryAddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStore.MakeGenericType(userType, roleType, contextType,
                    identityUserType.GenericTypeArguments[0],
                    identityUserType.GenericTypeArguments[1],
                    identityUserType.GenericTypeArguments[2],
                    identityUserType.GenericTypeArguments[3],
                    identityUserType.GenericTypeArguments[4],
                    identityRoleType.GenericTypeArguments[2]));
            services.TryAddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStore.MakeGenericType(roleType, contextType,
                    identityRoleType.GenericTypeArguments[0],
                    identityRoleType.GenericTypeArguments[1],
                    identityRoleType.GenericTypeArguments[2]));
        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType.GetTypeInfo();
            while (type.BaseType != null)
            {
                type = type.BaseType.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return type;
                }
            }
            return null;
        }
    }
}