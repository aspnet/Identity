// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Testing;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public abstract class SqlStoreTestBaseV1<TUser, TRole, TKey> : SqlStoreTestBase<TUser, TRole, TKey>, IClassFixture<ScratchDatabaseFixture>
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly ScratchDatabaseFixture _fixture;

        protected SqlStoreTestBaseV1(ScratchDatabaseFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

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
            VerifyDefaultSchema(CreateContext());
        }

        private static void VerifyDefaultSchema(TestDbContext dbContext)
        {
            var sqlConn = dbContext.Database.GetDbConnection();

            using (var db = new SqlConnection(sqlConn.ConnectionString))
            {
                db.Open();
                Assert.True(VerifyColumns(db, "AspNetUsers", "Id", "UserName", "Email", "PasswordHash", "SecurityStamp",
                    "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled",
                    "LockoutEnd", "AccessFailedCount", "ConcurrencyStamp", "NormalizedUserName", "NormalizedEmail"));
                Assert.True(VerifyColumns(db, "AspNetRoles", "Id", "Name", "NormalizedName", "ConcurrencyStamp"));
                Assert.True(VerifyColumns(db, "AspNetUserRoles", "UserId", "RoleId"));
                Assert.True(VerifyColumns(db, "AspNetUserClaims", "Id", "UserId", "ClaimType", "ClaimValue"));
                Assert.True(VerifyColumns(db, "AspNetUserLogins", "UserId", "ProviderKey", "LoginProvider", "ProviderDisplayName"));
                Assert.True(VerifyColumns(db, "AspNetUserTokens", "UserId", "LoginProvider", "Name", "Value"));

                VerifyIndex(db, "AspNetRoles", "RoleNameIndex", isUnique: true);
                VerifyIndex(db, "AspNetUsers", "UserNameIndex", isUnique: true);
                VerifyIndex(db, "AspNetUsers", "EmailIndex");
                db.Close();
            }
        }


    }
}