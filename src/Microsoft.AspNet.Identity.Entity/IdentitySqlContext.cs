// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityContext :
        IdentityContext<User, IdentityRole>
    {
        public IdentityContext() { }
        public IdentityContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IdentityContext(IServiceProvider serviceProvider, string nameOrConnectionString) : base(serviceProvider, nameOrConnectionString) { }
        public IdentityContext(DbContextOptions options) : base(options) { }
        public IdentityContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) { }
    }

    public class IdentityContext<TUser> :
        IdentityContext<TUser, IdentityRole>
        where TUser : User
    {
        public IdentityContext() { }
        public IdentityContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IdentityContext(IServiceProvider serviceProvider, string nameOrConnectionString) : base(serviceProvider, nameOrConnectionString) { }
        public IdentityContext(DbContextOptions options) : base(options) { }
        public IdentityContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) { }
    }

    public class IdentityContext<TUser, TRole> : DbContext
        where TUser : User
        where TRole : IdentityRole
    {
        public DbSet<TUser> Users { get; set; }
        public DbSet<IdentityUserClaim> UserClaims { get; set; }
        public DbSet<IdentityUserLogin> UserLogins { get; set; }
        public DbSet<IdentityUserRole> UserRoles { get; set; }
        public DbSet<TRole> Roles { get; set; }
        public DbSet<IdentityRoleClaim> RoleClaims { get; set; }

        private readonly string _nameOrConnectionString;

        public IdentityContext() { }
        public IdentityContext(IServiceProvider serviceProvider, string nameOrConnectionString) : base(serviceProvider)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }
        public IdentityContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IdentityContext(DbContextOptions options) : base(options) { }
        public IdentityContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options) { }

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

            builder.Entity<IdentityUserClaim>()
                .Key(uc => uc.Id)
                // TODO: causes issues with cascading deletes
                //.ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId))
                .ToTable("AspNetUserClaims");

            builder.Entity<IdentityUserRole>()
                .Key(r => new { r.UserId, r.RoleId })
                //.ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId))
                //.ForeignKeys(fk => fk.ForeignKey<TRole>(f => f.RoleId));
                .ToTable("AspNetUserRoles");

            builder.Entity<IdentityUserLogin>()
                .Key(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                //.ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId));
                .ToTable("AspNetUserLogins");
        }
    }
}