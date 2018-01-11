// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class UserStoreEncryptPersonalDataTest : SqlStoreTestBase<IdentityUser, IdentityRole, string>
    {
        public UserStoreEncryptPersonalDataTest(ScratchDatabaseFixture fixture)
            : base(fixture)
        { }

        protected override void SetupAddIdentity(IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Stores.EncryptPersonalData = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.AllowedUserNameCharacters = null;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<TestDbContext>()
            .AddPersonalDataEncryptor<SillyEncryptor>();
        }

        private class SillyEncryptor : IPersonalDataEncryptor
        {
            public string Decrypt(string data) => new string(data.Reverse().ToArray());

            public string Encrypt(string data) => new string(data.Reverse().ToArray());
        }
    }
}