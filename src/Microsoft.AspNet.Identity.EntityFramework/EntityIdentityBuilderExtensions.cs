// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace Microsoft.Framework.DependencyInjection
{
    public static class EntityIdentityBuilderExtensions
    {
        public static IdentityBuilder<User, IdentityRole> AddEntityFramework(this IdentityBuilder<User, IdentityRole> builder)
        {
            return AddEntityFramework<User, IdentityRole, IdentityDbContext>(builder);
        }

        public static IdentityBuilder<TUser, IdentityRole> AddEntityFramework<TUser, TContext>(this IdentityBuilder<TUser, IdentityRole> builder)
            where TUser : User where TContext : DbContext
        {
            return AddEntityFramework<TUser, IdentityRole, TContext>(builder);
        }

        public static IdentityBuilder<TUser, TRole> AddEntityFramework<TUser, TRole, TContext>(this IdentityBuilder<TUser, TRole> builder)
            where TUser : User
            where TRole : IdentityRole
            where TContext : DbContext
        {
            builder.Services.AddScoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext>>();
            builder.Services.AddScoped<IRoleStore<TRole>, RoleStore<TRole, TContext>>();
            builder.Services.AddScoped<TContext>();
            return builder;
        }
    }
}