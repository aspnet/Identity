// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity.SqlServer;
using Microsoft.AspNet.Identity.Test;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Redis.Extensions;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
using Xunit;

namespace Microsoft.AspNet.Identity.Redis.Test
{
    public class RedisTests : UserManagerTestBase<IdentityUser, IdentityRole>, IClassFixture<SimpleFixture>
    {
        private readonly DbContext _context;

        public RedisTests(SimpleFixture fixture)
        {
            _context = fixture.CreateContext();
        }


        protected override object CreateTestContext()
        {
            return _context;
        }


        protected override UserManager<IdentityUser> CreateManager(object context)
        {
            if (context == null)
            {
                context = _context;
            }
            var services = new ServiceCollection();
            services.AddEntityFramework().AddRedis();
            services.Add(OptionsServices.GetDefaultServices());
            services.AddIdentity();
            services.AddInstance<IUserStore<IdentityUser>>(new UserStore((DbContext)context));
            services.SetupOptions<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequireUppercase = false;
                options.User.AllowOnlyAlphanumericNames = false;
            });
            return services.BuildServiceProvider().GetService<UserManager<IdentityUser>>();
        }

        protected override RoleManager<IdentityRole> CreateRoleManager(object context)
        {
            if (context == null)
            {
                context = _context;
            }
            var services = new ServiceCollection();
            services.AddEntityFramework().AddRedis();
            services.AddIdentity();
            services.AddInstance<IRoleStore<IdentityRole>>(new RoleStore<IdentityRole>((DbContext)context));
            return services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();
        }

        [Fact]
        public void SimpleTest()
        {

        }
    }

    public class SimpleFixture
    {
        public DbContext CreateContext()
        {
            var options = new DbContextOptions()
                .UseModel(CreateModel())
                .UseRedis("127.0.0.1", 6379 /*RedisTestConfig.RedisPort*/);

            return new DbContext(options);
        }

        public IModel CreateModel()
        {
            var model = new Model();
            var builder = new BasicModelBuilder(model);
            builder.Entity<IdentityUser>(b =>
                {
                    b.Key(u => u.Id);
                    b.Property(u => u.UserName);
                    b.Property(u => u.PasswordHash);
                    b.Property(u => u.PhoneNumber);
                    b.Property(u => u.PhoneNumberConfirmed);
                    b.Property(u => u.NormalizedUserName);
                    b.Property(u => u.LockoutEnabled);
                    //b.Property(u => u.LockoutEnd);
                    b.Property(u => u.SecurityStamp);
                    b.Property(u => u.TwoFactorEnabled);
                    b.Property(u => u.Email);
                    b.Property(u => u.EmailConfirmed);
                });

            builder.Entity<IdentityRole>(b =>
                {
                    b.Key(r => r.Id);
                    b.Property(r => r.Name);
                });

            builder.Entity<IdentityUserRole<string>>(b =>
                {
                    b.Key(ur => new { ur.UserId, ur.RoleId });
                    b.ForeignKey<IdentityUser>(ur => ur.UserId);
                    b.ForeignKey<IdentityRole>(ur => ur.RoleId);
                });

            builder.Entity<IdentityUserLogin<string>>(b =>
                {
                    b.Key(ul => new { ul.LoginProvider, ul.ProviderKey });
                    b.Property(ul => ul.UserId);
                    b.ForeignKey<IdentityUser>(ul => ul.UserId);
                });

            builder.Entity<IdentityUserClaim<string>>(b =>
                {
                    b.Key(uc => uc.Id);
                    b.Property(uc => uc.ClaimType);
                    b.Property(uc => uc.ClaimValue);
                    b.ForeignKey<IdentityUser>(ul => ul.UserId);
                });

            builder.Entity<IdentityRoleClaim<string>>(b =>
                {
                    b.Key(rc => rc.Id);
                    b.Property(rc => rc.ClaimType);
                    b.Property(rc => rc.ClaimValue);
                    b.ForeignKey<IdentityRole>(rc => rc.RoleId);
                });

            return model;
        }
    }
}
