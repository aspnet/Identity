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
                options.Stores.ProtectPersonalData = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.AllowedUserNameCharacters = null;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<TestDbContext>()
            .AddPersonalDataEncryption<SillyEncryptor, DefaultKeyRing>();
        }


        public class DefaultKeyRing : IPersonalDataEncryptorKeyRing
        {
            public string this[string keyId] => keyId;
            public string CurrentKeyId => "Default";
        }

        private class SillyEncryptor : IPersonalDataEncryptor
        {
            private readonly IPersonalDataEncryptorKeyRing _keyRing;

            public SillyEncryptor(IPersonalDataEncryptorKeyRing keyRing) => _keyRing = keyRing;

            public string Decrypt(string keyId, string data)
            {
                var pad = _keyRing[keyId];
                if (!data.StartsWith(pad))
                {
                    throw new InvalidOperationException("Didn't find pad.");
                }
                return data.Substring(pad.Length);
            }

            public string Encrypt(string keyId, string data)
                => _keyRing[keyId] + data;
        }
    }
}