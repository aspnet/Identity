// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class UserStoreEncryptPersonalDataTest : SqlStoreTestBase<IdentityUser, IdentityRole, string>
    {
        private DefaultKeyRing _keyRing = new DefaultKeyRing();

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
            public static string Current = "Default";
            public string this[string keyId] => keyId;
            public string CurrentKeyId => Current;

            public IEnumerable<string> GetAllKeyIds()
            {
                return new string[] { "Default", "NewPad" };
            }
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

        /// <summary>
        /// Test.
        /// </summary>
        /// <returns>Task</returns>
        [Fact]
        public async Task CanRotateKeysAndStillFind()
        {
            if (ShouldSkipDbTests())
            {
                return;
            }
            var manager = CreateManager();
            var name = Guid.NewGuid().ToString();
            var user = CreateTestUser(name);
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
            IdentityResultAssert.IsSuccess(await manager.SetEmailAsync(user, "hao@hao.com"));
            var newName = Guid.NewGuid().ToString();
            Assert.Null(await manager.FindByNameAsync(newName));
            IdentityResultAssert.IsSuccess(await manager.SetPhoneNumberAsync(user, "123-456-7890"));

            Assert.Equal(user, await manager.FindByEmailAsync("hao@hao.com"));

            IdentityResultAssert.IsSuccess(await manager.SetUserNameAsync(user, newName));
            IdentityResultAssert.IsSuccess(await manager.UpdateAsync(user));
            Assert.NotNull(await manager.FindByNameAsync(newName));
            Assert.Null(await manager.FindByNameAsync(name));
            DefaultKeyRing.Current = "NewPad";
            Assert.NotNull(await manager.FindByNameAsync(newName));
            Assert.Equal(user, await manager.FindByEmailAsync("hao@hao.com"));
            Assert.Equal("123-456-7890", await manager.GetPhoneNumberAsync(user));
        }

    }
}