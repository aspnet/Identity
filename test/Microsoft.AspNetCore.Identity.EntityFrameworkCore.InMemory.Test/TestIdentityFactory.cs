// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.InMemory.Test
{
    public static class TestIdentityFactory
    {
        public static InMemoryContext CreateContext()
        {
            var services = new ServiceCollection();
            services.AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();

            var db = new InMemoryContext();
            //db.Database.EnsureCreated();

			throw new NotImplementedException();

            //return db;
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
            services.AddSingleton<IRoleStore<IdentityRole>>(new RoleStore<DataContext, DataConnection, IdentityRole>(new DefaultConnectionFactory<DataContext, DataConnection>()));
            return services.BuildServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        }

        public static RoleManager<IdentityRole> CreateRoleManager()
        {
            return CreateRoleManager(CreateContext());
        }
    }
}
