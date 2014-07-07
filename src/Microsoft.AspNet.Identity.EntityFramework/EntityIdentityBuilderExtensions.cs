// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using System;

namespace Microsoft.Framework.DependencyInjection
{
    public static class EntityIdentityBuilderExtensions
    {
        public static IdentityBuilder<IdentityUser, IdentityRole> AddEntityFramework(this IdentityBuilder<IdentityUser, IdentityRole> builder)
        {
            return AddEntityFramework<IdentityUser, IdentityRole, IdentityDbContext>(builder);
        }

        public static IdentityBuilder<IdentityUser, IdentityRole> AddEntityFramework<TContext>(this IdentityBuilder<IdentityUser, IdentityRole> builder)
            where TContext : DbContext
        {
            return AddEntityFramework<IdentityUser, IdentityRole, TContext>(builder);
        }

        public static IdentityBuilder<TUser, IdentityRole> AddEntityFramework<TUser, TContext>(this IdentityBuilder<TUser, IdentityRole> builder)
            where TUser : IdentityUser 
            where TContext : DbContext
        {
            return AddEntityFramework<TUser, IdentityRole, TContext>(builder);
        }

        public static IdentityBuilder<TUser, TRole> AddEntityFramework<TUser, TRole, TContext>(this IdentityBuilder<TUser, TRole> builder)
            where TUser : IdentityUser
            where TRole : IdentityRole
            where TContext : DbContext
        {
            builder.Services.AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext>>();
            builder.Services.AddScoped<IRoleStore<TRole>, RoleStore<TRole, TContext>>();
            builder.Services.AddScoped<TContext>();
            return builder;
        }

        public static IdentityBuilder<TUser, TRole> AddEntityFramework<TUser, TRole, TContext, TKey>(this IdentityBuilder<TUser, TRole> builder)
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TContext : DbContext
            where TKey : IEquatable<TKey>
        {
            builder.Services.AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext, TKey>>();
            builder.Services.AddScoped<IRoleStore<TRole>, RoleStore<TRole, TKey, TContext>>();
            builder.Services.AddScoped<TContext>();
            return builder;
        }

    }
}