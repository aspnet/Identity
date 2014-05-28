// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;

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

        public static UserManager<EntityUser> CreateManager(DbContext context)
        {
            var services = new ServiceCollection();
            services.AddTransient<IUserValidator<EntityUser>, UserValidator<EntityUser>>();
            services.AddTransient<IPasswordValidator<IdentityUser>, PasswordValidator<IdentityUser>>();
            services.AddInstance<IUserStore<EntityUser>>(new InMemoryUserStore<EntityUser>(context));
            services.AddSingleton<UserManager<EntityUser>>();
            services.SetupOptions<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequireUppercase = false;
                options.User.AllowOnlyAlphanumericNames = false;
            });
            services.AddSingleton<IOptionsAccessor<IdentityOptions>, OptionsAccessor<IdentityOptions>>();
            return services.BuildServiceProvider().GetService<UserManager<EntityUser>>();
        }

        public static UserManager<EntityUser> CreateManager()
        {
            return CreateManager(CreateContext());
        }

        public static RoleManager<EntityRole> CreateRoleManager(DbContext context)
        {
            var services = new ServiceCollection();
            services.AddTransient<IRoleValidator<EntityRole>, RoleValidator<EntityRole>>();
            services.AddInstance<IRoleStore<EntityRole>>(new EntityRoleStore<EntityRole, string>(context));
            return services.BuildServiceProvider().GetService<RoleManager<EntityRole>>();
        }

        public static RoleManager<EntityRole> CreateRoleManager()
        {
            return CreateRoleManager(CreateContext());
        }
    }
}
