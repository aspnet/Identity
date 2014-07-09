// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using System;

namespace Microsoft.Framework.DependencyInjection
{
    public static class EntityServiceCollectionExtensions
    {
        public static IdentityBuilder<IdentityUser, IdentityRole> AddIdentitySqlServer(this ServiceCollection services)
        {
            return services.AddIdentitySqlServer<IdentityDbContext, IdentityUser, IdentityRole>();
        }

        public static IdentityBuilder<IdentityUser, IdentityRole> AddIdentitySqlServer(this ServiceCollection services, Action<IdentityBuilder<IdentityUser, IdentityRole>> actionBuilder)
        {
            return services.AddIdentitySqlServer<IdentityDbContext, IdentityUser, IdentityRole>(actionBuilder);
        }

        public static IdentityBuilder<IdentityUser, IdentityRole> AddIdentitySqlServer<TContext>(this ServiceCollection services)
            where TContext : DbContext
        {
            return services.AddIdentitySqlServer<TContext, IdentityUser, IdentityRole>();
        }

        public static IdentityBuilder<IdentityUser, IdentityRole> AddIdentitySqlServer<TContext>(this ServiceCollection services, Action<IdentityBuilder<IdentityUser, IdentityRole>> actionBuilder)
            where TContext : DbContext
        {
            return services.AddIdentitySqlServer<TContext, IdentityUser, IdentityRole>(actionBuilder);
        }

        public static IdentityBuilder<TUser, IdentityRole> AddIdentitySqlServer<TContext, TUser>(this ServiceCollection services)
            where TUser : IdentityUser, new()
            where TContext : DbContext
        {
            return services.AddIdentitySqlServer<TContext, TUser, IdentityRole>();
        }

        public static IdentityBuilder<TUser, IdentityRole> AddIdentitySqlServer<TContext, TUser>(this ServiceCollection services, Action<IdentityBuilder<TUser, IdentityRole>> actionBuilder)
            where TUser : IdentityUser, new()
            where TContext : DbContext
        {
            return services.AddIdentitySqlServer<TContext, TUser, IdentityRole>(actionBuilder);
        }

        public static IdentityBuilder<TUser, TRole> AddIdentitySqlServer<TContext, TUser, TRole>(this ServiceCollection services)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TContext : DbContext
        {
            var builder = services.AddIdentity<TUser, TRole>();
            services.AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext>>();
            services.AddScoped<IRoleStore<TRole>, RoleStore<TRole, TContext>>();
            services.AddScoped<TContext>();
            return builder;
        }

        public static IdentityBuilder<TUser, TRole> AddIdentitySqlServer<TContext, TUser, TRole>(this ServiceCollection services, Action<IdentityBuilder<TUser, TRole>> actionBuilder)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TContext : DbContext
        {
            var builder = services.AddIdentitySqlServer<TContext, TUser, TRole>();
            actionBuilder(builder);
            return builder;
        }

        public static IdentityBuilder<TUser, TRole> AddIdentitySqlServer<TContext, TUser, TRole, TKey>(this ServiceCollection services)
            where TUser : IdentityUser<TKey>, new()
            where TRole : IdentityRole<TKey>, new()
            where TContext : DbContext
            where TKey : IEquatable<TKey>
        {
            var builder = services.AddIdentity<TUser, TRole>();
            services.AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext, TKey>>();
            services.AddScoped<IRoleStore<TRole>, RoleStore<TRole, TContext, TKey>>();
            services.AddScoped<TContext>();
            return builder;
        }

        public static IdentityBuilder<TUser, TRole> AddIdentitySqlServer<TContext, TUser, TRole, TKey>(this ServiceCollection services, Action<IdentityBuilder<TUser, TRole>> actionBuilder)
            where TUser : IdentityUser<TKey>, new()
            where TRole : IdentityRole<TKey>, new()
            where TContext : DbContext
            where TKey : IEquatable<TKey>
        {
            var builder = services.AddIdentitySqlServer<TContext, TUser, TRole, TKey>();
            actionBuilder(builder);
            return builder;
        }

    }
}