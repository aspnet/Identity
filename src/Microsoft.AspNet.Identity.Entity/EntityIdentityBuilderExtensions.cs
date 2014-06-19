// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity.Entity;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Identity
{
    public static class EntityIdentityBuilderExtensions
    {
        // todo: add overloads
        public static IdentityBuilder<TUser, IdentityRole> AddEntityFramework<TUser, TContext>(this IdentityBuilder<TUser, IdentityRole> builder)
            where TUser : User where TContext : DbContext
        {
            builder.Services.AddScoped<IUserStore<TUser>, UserStore<TUser, IdentityRole, TContext>>();
            builder.Services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole, TContext>>();
            builder.Services.AddScoped<TContext>();
            return builder;
        }
    }
}