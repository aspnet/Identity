// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Identity.EntityFramework.InMemory.Tests
{
    public static class TestIdentityFactory
    {
        public static InMemoryContext CreateContext()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework().AddInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();

            var db = new InMemoryContext();
            db.Database.EnsureCreated();

            return db;
        }

        public static IServiceCollection CreateTestServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddLogging();
            services.AddIdentity<IdentityUser, IdentityRole>();
            return services;
        }

        public static RoleManager<IdentityRole> CreateRoleManager(InMemoryContext context)
        {
            var services = CreateTestServices();
            services.AddInstance<IRoleStore<IdentityRole>>(new RoleStore<IdentityRole>(context));
            return services.BuildServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        }

        public static RoleManager<IdentityRole> CreateRoleManager()
        {
            return CreateRoleManager(CreateContext());
        }
    }
}
