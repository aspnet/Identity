// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public abstract class SqlStoreTestBaseV1<TUser, TRole, TKey> : SqlStoreTestBase<TUser, TRole, TKey>, IClassFixture<ScratchDatabaseFixture>
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected SqlStoreTestBaseV1(ScratchDatabaseFixture fixture) : base(fixture)
        { }

        public override TestDbContext CreateContext()
        {
            var db = base.CreateContext();
            db.Version = IdentityStoreOptions.Version1_0;
            return db;
        }

        protected override void AddUserStore(IServiceCollection services, object context = null)
        {
            services.AddSingleton<IUserStore<TUser>>(new UserStoreV1<TUser, TRole, TestDbContext, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>, IdentityRoleClaim<TKey>>((TestDbContext)context, new IdentityErrorDescriber()));
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            services.AddSingleton<IRoleStore<TRole>>(new RoleStoreV1<TRole, TestDbContext, TKey, IdentityUserRole<TKey>, IdentityRoleClaim<TKey>>((TestDbContext)context));
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public override void EnsureDefaultSchema()
        {
            var db = DbUtil.Create<TestDbContext>(_fixture.ConnectionString);
            var services = new ServiceCollection().AddSingleton(db);
            services.AddIdentity<TUser, TRole>().AddEntityFrameworkStoresLatest<TestDbContext>();
            var sp = services.BuildServiceProvider();
            var version = sp.GetRequiredService<IOptions<IdentityStoreOptions>>().Value.Version;
            Assert.Equal(IdentityStoreOptions.Version_Latest, version);
            db.Version = version;
            db.Database.EnsureCreated();
            VerifyDefaultSchema(db);
        }

        protected override void VerifyDefaultSchema(TestDbContext dbContext)
        {
            var sqlConn = dbContext.Database.GetDbConnection();

            using (var db = new SqlConnection(sqlConn.ConnectionString))
            {
                db.Open();
                VerifyColumns(db, "AspNetUsers", "Id", "UserName", "Email", "PasswordHash", "SecurityStamp",
                    "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled",
                    "LockoutEnd", "AccessFailedCount", "ConcurrencyStamp", "NormalizedUserName", "NormalizedEmail");
                VerifyColumns(db, "AspNetRoles", "Id", "Name", "NormalizedName", "ConcurrencyStamp");
                VerifyColumns(db, "AspNetUserRoles", "UserId", "RoleId");
                VerifyColumns(db, "AspNetUserClaims", "Id", "UserId", "ClaimType", "ClaimValue");
                VerifyColumns(db, "AspNetUserLogins", "UserId", "ProviderKey", "LoginProvider", "ProviderDisplayName");
                VerifyColumns(db, "AspNetUserTokens", "UserId", "LoginProvider", "Name", "Value");

                VerifyIndex(db, "AspNetRoles", "RoleNameIndex", isUnique: true);
                VerifyIndex(db, "AspNetUsers", "UserNameIndex", isUnique: true);
                VerifyIndex(db, "AspNetUsers", "EmailIndex");
                db.Close();
            }
        }


    }
}