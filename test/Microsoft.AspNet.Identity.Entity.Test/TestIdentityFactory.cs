// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity.Test;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;

namespace Microsoft.AspNet.Identity.Entity.Test
{
    public static class TestIdentityFactory
    {
        public static IdentityContext CreateContext()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework().AddInMemoryStore();
            var serviceProvider = services.BuildServiceProvider();

            var db = new IdentityContext(serviceProvider);
            db.Database.EnsureCreated();

            return db;
        }

        public static UserManager<EntityUser> CreateManager(IdentityContext context)
        {
            return MockHelpers.CreateManager<EntityUser>(() => new InMemoryUserStore<EntityUser>(context));
        }

        public static UserManager<EntityUser> CreateManager()
        {
            return CreateManager(CreateContext());
        }

        public static RoleManager<IdentityRole> CreateRoleManager(IdentityContext context)
        {
            var services = new ServiceCollection();
            services.AddIdentity<EntityUser, IdentityRole>(b => b.AddRoleStore(() => new RoleStore<IdentityRole>(context)));
            return services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();
        }

        public static RoleManager<IdentityRole> CreateRoleManager()
        {
            return CreateRoleManager(CreateContext());
        }
    }
}
