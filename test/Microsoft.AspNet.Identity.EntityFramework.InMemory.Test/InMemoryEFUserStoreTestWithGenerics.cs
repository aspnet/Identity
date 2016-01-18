// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Test;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNet.Testing.xunit;
using Xunit;
using System.Threading;

namespace Microsoft.AspNet.Identity.EntityFramework.InMemory.Test
{
    using System.Collections.Generic;
    using Microsoft.Data.Entity;
    using System.Linq;

    public class InMemoryEFUserStoreTestWithGenerics : UserManagerTestBase<IdentityUserWithGenerics, MyIdentityRole, string>, IDisposable
    {
        InMemoryContextWithGenerics inMemoryDb;

        public InMemoryEFUserStoreTestWithGenerics()
        {
            inMemoryDb = new InMemoryContextWithGenerics();
        }
        public void Dispose()
        {
            inMemoryDb.Dispose();
        }

        protected override object CreateTestContext()
        {
            return inMemoryDb;
//            return new InMemoryContextWithGenerics();
        }

        private UserStoreWithGenerics currentUserStore;

        protected override void AddUserStore(IServiceCollection services, object context = null)
        {
            this.currentUserStore = new UserStoreWithGenerics((InMemoryContextWithGenerics)context, "TestContext");
            services.AddSingleton<IUserStore<IdentityUserWithGenerics>>(this.currentUserStore);
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            var store = new RoleStoreWithGenerics((InMemoryContextWithGenerics)context, "TestContext");
            services.AddSingleton<IRoleStore<MyIdentityRole>>(store);
        }

        protected override IdentityUserWithGenerics CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
            bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = default(DateTimeOffset?), bool useNamePrefixAsUserName = false)
        {
            return new IdentityUserWithGenerics
            {
                UserName = useNamePrefixAsUserName ? namePrefix : string.Format("{0}{1}", namePrefix, Guid.NewGuid()),
                Email = email,
                PhoneNumber = phoneNumber,
                LockoutEnabled = lockoutEnabled,
                LockoutEnd = lockoutEnd
            };
        }

        protected override MyIdentityRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false)
        {
            var roleName = useRoleNamePrefixAsRoleName ? roleNamePrefix : string.Format("{0}{1}", roleNamePrefix, Guid.NewGuid());
            return new MyIdentityRole(roleName);
        }

        protected override void SetUserPasswordHash(IdentityUserWithGenerics user, string hashedPassword)
        {
            user.PasswordHash = hashedPassword;
        }


        protected override Expression<Func<IdentityUserWithGenerics, bool>> UserNameEqualsPredicate(string userName) => u => u.UserName == userName;

        protected override Expression<Func<MyIdentityRole, bool>> RoleNameEqualsPredicate(string roleName) => r => r.Name == roleName;

        protected override Expression<Func<IdentityUserWithGenerics, bool>> UserNameStartsWithPredicate(string userName) => u => u.UserName.StartsWith(userName);

        protected override Expression<Func<MyIdentityRole, bool>> RoleNameStartsWithPredicate(string roleName) => r => r.Name.StartsWith(roleName);

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public async Task CanAddRemoveUserClaimWithIssuer()
        {
            var manager = CreateManager();
            var user = CreateTestUser();
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
            Claim[] claims = { new Claim("c1", "v1", null, "i1"), new Claim("c2", "v2", null, "i2"), new Claim("c2", "v3", null, "i3") };
            foreach (Claim c in claims)
            {
                IdentityResultAssert.IsSuccess(await manager.AddClaimAsync(user, c));
            }

            var userId = await manager.GetUserIdAsync(user);
            var userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(3, userClaims.Count);
            Assert.Equal(3, userClaims.Intersect(claims, ClaimEqualityComparer.Default).Count());

            IdentityResultAssert.IsSuccess(await manager.RemoveClaimAsync(user, claims[0]));
            userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(2, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaimAsync(user, claims[1]));
            userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(1, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaimAsync(user, claims[2]));
            userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(0, userClaims.Count);
        }


        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public async Task RemoveClaimWithIssuerOnlyAffectsUser()
        {
            var manager = CreateManager();
            var user = CreateTestUser();
            var user2 = CreateTestUser();
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user2));
            Claim[] claims = { new Claim("c", "v", null, "i1"), new Claim("c2", "v2", null, "i2"), new Claim("c2", "v3", null, "i3") };
            foreach (Claim c in claims)
            {
                IdentityResultAssert.IsSuccess(await manager.AddClaimAsync(user, c));
                IdentityResultAssert.IsSuccess(await manager.AddClaimAsync(user2, c));
            }
            var userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(3, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaimAsync(user, claims[0]));
            userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(2, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaimAsync(user, claims[1]));
            userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(1, userClaims.Count);
            IdentityResultAssert.IsSuccess(await manager.RemoveClaimAsync(user, claims[2]));
            userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(0, userClaims.Count);
            var userClaims2 = await manager.GetClaimsAsync(user2);
            Assert.Equal(3, userClaims2.Count);
        }


        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public async Task CanReplaceUserClaimWithIssuer()
        {
            var manager = CreateManager();
            var user = CreateTestUser();
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
            IdentityResultAssert.IsSuccess(await manager.AddClaimAsync(user, new Claim("c", "a", "i")));
            var userClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(1, userClaims.Count);
            Claim claim = new Claim("c", "b", "i");
            Claim oldClaim = userClaims.FirstOrDefault();
            IdentityResultAssert.IsSuccess(await manager.ReplaceClaimAsync(user, oldClaim, claim));
            var newUserClaims = await manager.GetClaimsAsync(user);
            Assert.Equal(1, newUserClaims.Count);
            Claim newClaim = newUserClaims.FirstOrDefault();
            Assert.Equal(claim.Type, newClaim.Type);
            Assert.Equal(claim.Value, newClaim.Value);
            Assert.Equal(claim.Issuer, newClaim.Issuer);
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public async Task CanUseDifferentUsersFromSameExternalProviderWhenUsingContext()
        {
            var manager = CreateManager();
            var user1 = CreateTestUser();
            var user2 = CreateTestUser();
            var loginInfo = new UserLoginInfo("MyProvider", Guid.NewGuid().ToString(), "MyProvider");

            var firstContext = this.currentUserStore.LoginContext;
            var secondContext = "TestContext2";

            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user1));
            IdentityResultAssert.IsSuccess(await manager.AddLoginAsync(user1, loginInfo));
            IdentityResultAssert.IsFailure(await manager.AddLoginAsync(user1, loginInfo));

            this.currentUserStore.LoginContext = secondContext;
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user2));
            IdentityResultAssert.IsSuccess(await manager.AddLoginAsync(user2, loginInfo));

            this.currentUserStore.LoginContext = firstContext;
            var foundUser = await manager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            Assert.Equal(user1.Id, foundUser.Id);

            this.currentUserStore.LoginContext = secondContext;
            foundUser = await manager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            Assert.Equal(user2.Id, foundUser.Id);
        }

    }

    internal class ClaimEqualityComparer : IEqualityComparer<Claim>
    {
        public static IEqualityComparer<Claim> Default = new ClaimEqualityComparer();

        public bool Equals(Claim x, Claim y)
        {
            return x.Value == y.Value && x.Type == y.Type && x.Issuer == y.Issuer;
        }

        public int GetHashCode(Claim obj)
        {
            return 1;
        }
    }


    #region Generic Type defintions

    public class IdentityUserWithGenerics : IdentityUser<string, IdentityUserClaimWithIssuer, IdentityUserRoleWithDate, IdentityUserLoginWithContext>
    {
        public IdentityUserWithGenerics()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }

    public class UserStoreWithGenerics : UserStore<IdentityUserWithGenerics, MyIdentityRole, InMemoryContextWithGenerics, string, IdentityUserClaimWithIssuer, IdentityUserRoleWithDate, IdentityUserLoginWithContext>
    {
        public string LoginContext { get; set; }

        public UserStoreWithGenerics(InMemoryContextWithGenerics context, string loginContext) : base(context)
        {
            this.LoginContext = loginContext;
        }

        protected override Task<IdentityUserLoginWithContext> FindUserLoginAsync(IdentityUserWithGenerics user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return this.UserLogins.FirstOrDefaultAsync(userLogin => userLogin.Context == this.LoginContext && userLogin.UserId.Equals(user.Id) && userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey, cancellationToken);
        }

        protected override Task<IdentityUserLoginWithContext> FindLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return this.UserLogins.FirstOrDefaultAsync(userLogin => userLogin.Context == this.LoginContext && userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey, cancellationToken);
        }

        protected override IdentityUserRoleWithDate OnCreateUserRole(IdentityUserWithGenerics user, MyIdentityRole role)
        {
            return new IdentityUserRoleWithDate()
            {
                RoleId = role.Id,
                UserId = user.Id,
                Created = DateTime.UtcNow
            };
        }

        protected override Task<bool> IsInRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            return this.UserRoles.AnyAsync(ur => ur.RoleId.Equals(roleId) && ur.UserId.Equals(userId), cancellationToken);
        }

        protected override Task<IdentityUserRoleWithDate> FindUserRoleAsync(IdentityUserWithGenerics user, MyIdentityRole roleEntity, CancellationToken cancellationToken)
        {
            return this.UserRoles.FirstOrDefaultAsync(r => roleEntity.Id.Equals(r.RoleId) && r.UserId.Equals(user.Id), cancellationToken);
        }

        protected override IdentityUserClaimWithIssuer OnCreateUserClaim(IdentityUserWithGenerics user, Claim claim)
        {
            return new IdentityUserClaimWithIssuer { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value, Issuer = claim.Issuer };
        }

        protected override Task<List<IdentityUserClaimWithIssuer>> FindUserClaimsAsync(IdentityUserWithGenerics user, Claim claim, CancellationToken cancellationToken)
        {
            return this.UserClaims.Where(uc => uc.Issuer == claim.Issuer && uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToListAsync(cancellationToken);
        }

        protected override IdentityUserLoginWithContext OnCreateUserLogin(IdentityUserWithGenerics user, UserLoginInfo login)
        {
            return new IdentityUserLoginWithContext
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                Context = this.LoginContext
            };
        }
    }

    public class RoleStoreWithGenerics : RoleStore<MyIdentityRole, InMemoryContextWithGenerics, string, IdentityUserRoleWithDate, IdentityRoleClaim<string>>
    {
        string loginContext;
        public RoleStoreWithGenerics(InMemoryContextWithGenerics context, string loginContext) : base(context)
        {
            this.loginContext = loginContext;
        }

        protected async override Task<List<IdentityRoleClaim<string>>> FindRoleClaimsAsync(MyIdentityRole role, Claim claim, CancellationToken cancellationToken)
        {
            return await this.RoleClaims.Where(rc => rc.RoleId.Equals(role.Id) && rc.ClaimValue == claim.Value && rc.ClaimType == claim.Type).ToListAsync(cancellationToken);
        }

        protected override IdentityRoleClaim<string> OnCreateRoleClaim(MyIdentityRole role, Claim claim)
        {
            return new IdentityRoleClaim<string> { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
        }
    }

    public class IdentityUserClaimWithIssuer : IdentityUserClaim<string>
    {
        public string Issuer { get; set; }

        public override Claim ToClaim()
        {
            return new Claim(this.ClaimType, this.ClaimValue, null, this.Issuer);
        }

        public override void FromClaim(Claim other)
        {
            this.ClaimValue = other.Value;
            this.ClaimType = other.Type;
            this.Issuer = other.Issuer;
        }
    }

    public class IdentityUserRoleWithDate : IdentityUserRole<string>
    {
        public DateTime Created { get; set; }
    }

    public class MyIdentityRole : IdentityRole<string, IdentityUserRoleWithDate, IdentityRoleClaim<string>>
    {
        public MyIdentityRole() : base()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public MyIdentityRole(string roleName) : this()
        {
            this.Name = roleName;
        }

    }

    public class IdentityUserLoginWithContext : IdentityUserLogin<string>
    {
        public string Context { get; set; }
    }

    public class InMemoryContextWithGenerics : InMemoryContext<IdentityUserWithGenerics, MyIdentityRole, string, IdentityUserClaimWithIssuer, IdentityUserRoleWithDate, IdentityUserLoginWithContext, IdentityRoleClaim<string>>
    {
        public InMemoryContextWithGenerics()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase();
        }

        protected override void OnBuildUser(EntityTypeBuilder<IdentityUserWithGenerics> builder)
        {
        }

        protected override void OnBuildRole(EntityTypeBuilder<MyIdentityRole> builder)
        {
        }

        protected override void OnBuildUserClaim(EntityTypeBuilder<IdentityUserClaimWithIssuer> builder)
        {
            builder.HasKey(uc => uc.Id);
        }

        protected override void OnBuildRoleClaim(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.HasKey(rc => rc.Id);
        }
        
        protected override void OnBuildUserRole(EntityTypeBuilder<IdentityUserRoleWithDate> builder)
        {
            builder.HasKey(r => new { r.UserId, r.RoleId });
        }

        protected override void OnBuildUserLogin(EntityTypeBuilder<IdentityUserLoginWithContext> builder)
        {
            builder.HasKey(l => new { l.LoginProvider, l.ProviderKey, l.Context });
        }
    }

    #endregion
}
