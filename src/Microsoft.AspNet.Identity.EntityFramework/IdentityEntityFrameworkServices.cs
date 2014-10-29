// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    /// Default services
    /// </summary>
    public class IdentityEntityFrameworkServices
    {
        public static IEnumerable<IServiceDescriptor> GetDefaultServices<TContext, TUser, TRole>(IConfiguration config = null)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TContext : DbContext
        {
            ServiceDescriber describe;
            if (config == null)
            {
                describe = new ServiceDescriber();
            }
            else
            {
                describe = new ServiceDescriber(config);
            }
            yield return describe.Scoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext>>();
            yield return describe.Scoped<IRoleStore<TRole>, RoleStore<TRole, TContext>>();
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultServices<TContext, TUser, TRole, TKey>(IConfiguration config = null)
            where TUser : IdentityUser<TKey>, new()
            where TRole : IdentityRole<TKey>, new()
            where TContext : DbContext
            where TKey : IEquatable<TKey>
        {
            ServiceDescriber describe;
            if (config == null)
            {
                describe = new ServiceDescriber();
            }
            else
            {
                describe = new ServiceDescriber(config);
            }
            yield return describe.Scoped<IUserStore<TUser>, UserStore<TUser, TRole, TContext, TKey>>();
            yield return describe.Scoped<IRoleStore<TRole>, RoleStore<TRole, TContext, TKey>>();
        }

    }
}