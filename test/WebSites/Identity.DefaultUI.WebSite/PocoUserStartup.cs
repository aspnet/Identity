// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.InMemory;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.DefaultUI.WebSite
{
    public class PocoUserStartup : StartupBase<PocoUser, IdentityDbContext>
    {
        public PocoUserStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            services.AddDefaultIdentity<TestUser>()
                .AddUserManager<UserManager<TestUser>>();
            services.AddSingleton<IUserStore<TestUser>, InMemoryUserStore<TestUser>>();
            services.AddSingleton<IUserFactory<TestUser>, TestUserFactory>();

            services.AddMvc();
        }

        public class TestUserFactory : IUserFactory<TestUser>
        {
            public TestUser CreateUser(string email, string userName)
            {
                return new TestUser
                {
                    Email = email,
                    UserName = userName
                };
            }
        }
    }
}
