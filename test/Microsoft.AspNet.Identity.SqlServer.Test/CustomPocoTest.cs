// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Test;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Services;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Xunit;

namespace Microsoft.AspNet.Identity.SqlServer.Test
{
    [TestCaseOrderer("Microsoft.AspNet.Identity.Test.PriorityOrderer", "Microsoft.AspNet.Identity.SqlServer.Test")]
    public class CustomPocoTest
    {
        private readonly string ConnectionString = @"Server=(localdb)\v11.0;Database=CustomUserContextTest" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + ";Trusted_Connection=True;";

        public class User<TKey> where TKey : IEquatable<TKey>
        {
            public TKey Id { get; set; }
            public string UserName { get; set; }
        }

        public class CustomDbContext<TKey> : DbContext where TKey : IEquatable<TKey>
        {
            public DbSet<User<TKey>> Users { get; set; }

            public CustomDbContext(IServiceProvider services) :
                base(services, services.GetService<IOptionsAccessor<DbContextOptions>>().Options)
            { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<User<TKey>>().Property(p => p.Id)
                            .GenerateValuesOnAdd(generateValues: false);
            }
        }


        public CustomDbContext<TKey> GetContext<TKey>() where TKey : IEquatable<TKey>
        {
            var services = new ServiceCollection();
            services.Add(OptionsServices.GetDefaultServices());
            services.AddInstance<ILoggerFactory>(new NullLoggerFactory());
            services.AddEntityFramework().AddSqlServer();
            services.ConfigureOptions<DbContextOptions>(options => options.UseSqlServer(ConnectionString));
            var serviceProvider = services.BuildServiceProvider();
            return new CustomDbContext<TKey>(serviceProvider);
        }

        public CustomDbContext<TKey> CreateContext<TKey>(bool delete = false) where TKey : IEquatable<TKey>
        {
            var db = GetContext<TKey>();
            if (delete)
            {
                db.Database.EnsureDeleted();
            }
            db.Database.EnsureCreated();
            return db;
        }

        [TestPriority(-1000)]
        [Fact]
        public void DropDatabaseStart()
        {
            DropDb();
        }

        [TestPriority(10000)]
        [Fact]
        public void DropDatabaseDone()
        {
            DropDb();
        }

        public void DropDb()
        {
            var db = GetContext<string>();
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async Task CanUpdateNameGuid()
        {
            using (var db = CreateContext<Guid>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<Guid> { UserName = oldName, Id = Guid.NewGuid() };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                var newName = Guid.NewGuid().ToString();
                user.UserName = newName;
                await db.SaveChangesAsync();
                Assert.Null(db.Users.SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Users.Single(u => u.UserName == newName));
            }
        }

        [Fact]
        public async Task CanUpdateNameString()
        {
            using (var db = CreateContext<string>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<string> { UserName = oldName, Id = Guid.NewGuid().ToString() };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                var newName = Guid.NewGuid().ToString();
                user.UserName = newName;
                await db.SaveChangesAsync();
                Assert.Null(db.Users.SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Users.Single(u => u.UserName == newName));
            }
        }

        [Fact]
        public async Task CanCreateUserInt()
        {
            using (var db = CreateContext<int>(true))
            {
                var user = new User<int> { Id = 11 };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                user.UserName = "Boo";
                await db.SaveChangesAsync();
                var fetch = db.Users.First(u => u.UserName == "Boo");
                Assert.Equal(user, fetch);
            }
        }

        [Fact]
        public async Task CanCreateUserIntViaSet()
        {
            using (var db = CreateContext<int>(true))
            {
                var user = new User<int> { Id = 11 };
                var users = db.Set<User<int>>();
                users.Add(user);
                await db.SaveChangesAsync();
                user.UserName = "Boo";
                await db.SaveChangesAsync();
                var fetch = users.First(u => u.UserName == "Boo");
                Assert.Equal(user, fetch);
            }
        }

        [Fact]
        public async Task CanUpdateNameInt()
        {
            using (var db = CreateContext<int>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<int> { UserName = oldName, Id = 1 };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                var newName = Guid.NewGuid().ToString();
                user.UserName = newName;
                await db.SaveChangesAsync();
                Assert.Null(db.Users.SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Users.Single(u => u.UserName == newName));
            }
        }

        [Fact]
        public async Task CanUpdateNameIntWithSet()
        {
            using (var db = CreateContext<int>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<int> { UserName = oldName, Id = 1 };
                db.Set<User<int>>().Add(user);
                await db.SaveChangesAsync();
                var newName = Guid.NewGuid().ToString();
                user.UserName = newName;
                await db.SaveChangesAsync();
                Assert.Null(db.Set<User<int>>().SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Set<User<int>>().Single(u => u.UserName == newName));
            }
        }
    }
}