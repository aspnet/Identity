// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Base class for the Entity Framework database context used for identity.
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<IdentityUser, IdentityRole, string> { }

    /// <summary>
    /// Base class for the Entity Framework database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type of the user objects.</typeparam>
    public class IdentityDbContext<TUser> : IdentityDbContext<TUser, IdentityRole, string> where TUser : IdentityUser
    { }

    /// <summary>
    /// Base class for the Entity Framework database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <typeparam name="TRole">The type of role objects.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    public class IdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityDbContext"/>.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public IdentityDbContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext" /> class using an <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="serviceProvider"> The service provider to be used.</param>
        public IdentityDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext" /> class using an <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        /// <param name="serviceProvider"> The service provider to be used.</param>
        public IdentityDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext" /> class.
        /// </summary>
        protected IdentityDbContext()
        {

        }
        
        protected override void OnBuildUser(EntityTypeBuilder<TUser> builder)
        {
        }
        
        protected override void OnBuildRole(EntityTypeBuilder<TRole> builder)
        {
        }
        
        protected override void OnBuildUserClaim(EntityTypeBuilder<IdentityUserClaim<TKey>> builder)
        {
            builder.HasKey(uc => uc.Id);
        }
        
        protected override void OnBuildRoleClaim(EntityTypeBuilder<IdentityRoleClaim<TKey>> builder)
        {
            builder.HasKey(rc => rc.Id);
        }
        
        protected override void OnBuildUserRole(EntityTypeBuilder<IdentityUserRole<TKey>> builder)
        {
            builder.HasKey(r => new { r.UserId, r.RoleId });
        }

        protected override void OnBuildUserLogin(EntityTypeBuilder<IdentityUserLogin<TKey>> builder)
        {
            builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });
        }
    }

    /// <summary>
    /// Base class for the Entity Framework database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <typeparam name="TRole">The type of role objects.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    /// <typeparam name="TUserClaim">The type of the user claim object.</typeparam>
    /// <typeparam name="TUserRole">The type of the user role object.</typeparam>
    /// <typeparam name="TUserLogin">The type of the user login object.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the role claim object.</typeparam>
    public abstract class IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim> : DbContext
        where TUser : IdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
        where TRole : IdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim: IdentityRoleClaim<TKey>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityDbContext"/>.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public IdentityDbContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext" /> class using an <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="serviceProvider"> The service provider to be used.</param>
        public IdentityDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext" /> class using an <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        /// <param name="serviceProvider"> The service provider to be used.</param>
        public IdentityDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(serviceProvider, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext" /> class.
        /// </summary>
        protected IdentityDbContext()
        {

        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of Users.
        /// </summary>
        public DbSet<TUser> Users { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of User claims.
        /// </summary>
        public DbSet<TUserClaim> UserClaims { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of User logins.
        /// </summary>
        public DbSet<TUserLogin> UserLogins { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of User roles.
        /// </summary>
        public DbSet<TUserRole> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of roles.
        /// </summary>
        public DbSet<TRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of role claims.
        /// </summary>
        public DbSet<TRoleClaim> RoleClaims { get; set; }

        /// <summary>
        /// Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TUser>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex");
                b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
                b.ToTable("AspNetUsers");
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                b.HasMany(u => u.Claims).WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany(u => u.Logins).WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany(u => u.Roles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

                OnBuildUser(b);
            });

            builder.Entity<TRole>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex");
                b.ToTable("AspNetRoles");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany(r => r.Users).WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany(r => r.Claims).WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();

                OnBuildRole(b);
            });

            builder.Entity<TUserClaim>(b =>
            {
                b.ToTable("AspNetUserClaims");
                OnBuildUserClaim(b);
            });

            builder.Entity<TRoleClaim>(b =>
            {
                b.ToTable("AspNetRoleClaims");
                OnBuildRoleClaim(b);
            });

            builder.Entity<TUserRole>(b =>
            {
                b.ToTable("AspNetUserRoles");
                OnBuildUserRole(b);
            });

            builder.Entity<TUserLogin>(b =>
            {
                b.ToTable("AspNetUserLogins");
                OnBuildUserLogin(b);
            });
        }

        /// <summary>
        /// Allow further customization of the <typeparamref name="TRole"/> schema
        /// </summary>
        /// <param name="builder">Entity type builder of <typeparamref name="TRole"/></param>
        protected abstract void OnBuildRole(EntityTypeBuilder<TRole> builder);

        /// <summary>
        /// Allow further customization of the <typeparamref name="TUser"/> schema
        /// </summary>
        /// <param name="builder">Entity type builder of <typeparamref name="TUser"/></param>
        protected abstract void OnBuildUser(EntityTypeBuilder<TUser> builder);

        /// <summary>
        /// Allow further customization of the <typeparamref name="TUserClaim"/> schema
        /// </summary>
        /// <param name="builder">Entity type builder of <typeparamref name="TUserClaim"/></param>
        protected abstract void OnBuildUserClaim(EntityTypeBuilder<TUserClaim> builder);

        /// <summary>
        /// Allow further customization of the <typeparamref name="TRoleClaim"/> schema
        /// </summary>
        /// <param name="builder">Entity type builder of <typeparamref name="TRoleClaim"/></param>
        protected abstract void OnBuildRoleClaim(EntityTypeBuilder<TRoleClaim> builder);

        /// <summary>
        /// Allow further customization of the <typeparamref name="TUserRole"/> schema
        /// </summary>
        /// <param name="builder">Entity type builder of <typeparamref name="TUserRole"/></param>
        protected abstract void OnBuildUserRole(EntityTypeBuilder<TUserRole> builder);

        /// <summary>
        /// Allow further customization of the <typeparamref name="TUserLogin"/> schema
        /// </summary>
        /// <param name="builder">Entity type builder of <typeparamref name="TUserLogin"/></param>
        protected abstract void OnBuildUserLogin(EntityTypeBuilder<TUserLogin> builder);
    }
}