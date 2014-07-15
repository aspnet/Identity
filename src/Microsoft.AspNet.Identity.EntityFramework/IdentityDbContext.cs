// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class IdentityDbContext :
        IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public IdentityDbContext() { }
        public IdentityDbContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IdentityDbContext(IServiceProvider serviceProvider, string nameOrConnectionString) : base(serviceProvider, nameOrConnectionString) { }
        public IdentityDbContext(DbContextOptions options) : base(options) { }
        public IdentityDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) { }
    }

    public class IdentityDbContext<TUser> :
        IdentityDbContext<TUser, IdentityRole, string>
        where TUser : IdentityUser
    {
        public IdentityDbContext() { }
        public IdentityDbContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IdentityDbContext(IServiceProvider serviceProvider, string nameOrConnectionString) : base(serviceProvider, nameOrConnectionString) { }
        public IdentityDbContext(DbContextOptions options) : base(options) { }
        public IdentityDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) { }
    }

    public class IdentityDbContext<TUser, TRole, TKey> : DbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public DbSet<TUser> Users { get; set; }
        public DbSet<IdentityUserClaim<TKey>> UserClaims { get; set; }
        public DbSet<IdentityUserLogin<TKey>> UserLogins { get; set; }
        public DbSet<IdentityUserRole<TKey>> UserRoles { get; set; }
        public DbSet<TRole> Roles { get; set; }
        public DbSet<IdentityRoleClaim<TKey>> RoleClaims { get; set; }

        private readonly string _nameOrConnectionString;

        public IdentityDbContext() { }
        public IdentityDbContext(IServiceProvider serviceProvider, string nameOrConnectionString) : base(serviceProvider)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }
        public IdentityDbContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IdentityDbContext(DbContextOptions options) : base(options) { }
        public IdentityDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) { }

        protected override void OnConfiguring(DbContextOptions builder)
        {
            if (!string.IsNullOrEmpty(_nameOrConnectionString))
            {
                builder.UseSqlServer(_nameOrConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TUser>()
                .Key(u => u.Id)
                .Properties(ps => ps.Property(u => u.UserName))
                .ToTable("AspNetUsers");

            builder.Entity<TRole>()
                .Key(r => r.Id)
                .Properties(ps => ps.Property(r => r.Name))
                .ToTable("AspNetRoles");

            builder.Entity<IdentityUserClaim<TKey>>()
                .Key(uc => uc.Id)
                .ToTable("AspNetUserClaims");

            builder.Entity<IdentityRoleClaim<TKey>>()
                .Key(uc => uc.Id)
                .ToTable("AspNetRoleClaims");

            var userType = builder.Model.GetEntityType(typeof(TUser));
            var roleType = builder.Model.GetEntityType(typeof(TRole));
            var userClaimType = builder.Model.GetEntityType(typeof(IdentityUserClaim<TKey>));
            var roleClaimType = builder.Model.GetEntityType(typeof(IdentityRoleClaim<TKey>));
            var userRoleType = builder.Model.GetEntityType(typeof(IdentityUserRole<TKey>));
            var ucfk = userClaimType.AddForeignKey(userType.GetKey(), new[] { userClaimType.GetProperty("UserId") });
            userType.AddNavigation(new Navigation(ucfk, "Claims", false));
            //userClaimType.AddNavigation(new Navigation(ucfk, "User", true));
            //var urfk = userRoleType.AddForeignKey(userType.GetKey(), new[] { userRoleType.GetProperty("UserId") });
            //userType.AddNavigation(new Navigation(urfk, "Roles", false));

            //var urfk2 = userRoleType.AddForeignKey(roleType.GetKey(), new[] { userRoleType.GetProperty("RoleId") });
            //roleType.AddNavigation(new Navigation(urfk2, "Users", false));

            var rcfk = roleClaimType.AddForeignKey(roleType.GetKey(), new[] { roleClaimType.GetProperty("RoleId") });
            roleType.AddNavigation(new Navigation(rcfk, "Claims", false));

            builder.Entity<IdentityUserRole<TKey>>()
                .Key(r => new { r.UserId, r.RoleId })
                // Blocks delete currently without cascade
                //.ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId))
                //.ForeignKeys(fk => fk.ForeignKey<TRole>(f => f.RoleId));
                .ToTable("AspNetUserRoles");

            builder.Entity<IdentityUserLogin<TKey>>()
                .Key(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                //.ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId))
                .ToTable("AspNetUserLogins");
        }
    }
}