// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Identity.Test;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
using Microsoft.AspNet.Hosting;

namespace Microsoft.AspNet.Identity.InMemory.Test
{
    public class InMemoryStoreTest : UserManagerTestBase<IdentityUser, IdentityRole>
    {
        protected override object CreateTestContext()
        {
            return null;
        }

        protected override UserManager<IdentityUser> CreateManager(object context)
        {
            var services = new ServiceCollection();
            services.Add(OptionsServices.GetDefaultServices());
            services.Add(HostingServices.GetDefaultServices());
            services.AddDefaultIdentity<IdentityUser, IdentityRole>();
            services.AddSingleton<IUserStore<IdentityUser>, InMemoryUserStore<IdentityUser>>();
            services.AddSingleton<IRoleStore<IdentityRole>, InMemoryRoleStore<IdentityRole>>();
            services.ConfigureIdentity(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequireUppercase = false;
                options.User.UserNameValidationRegex = null;
            });
            return services.BuildServiceProvider().GetRequiredService<UserManager<IdentityUser>>();
        }

        protected override RoleManager<IdentityRole> CreateRoleManager(object context)
        {
            var services = new ServiceCollection();
            services.AddIdentity();
            services.AddSingleton<IRoleStore<IdentityRole>, InMemoryRoleStore<IdentityRole>>();
            return services.BuildServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        }
    }
}