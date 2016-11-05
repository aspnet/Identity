// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class CustomPocoTest : IClassFixture<ScratchDatabaseFixture>
    {
        private readonly ScratchDatabaseFixture _fixture;

        public CustomPocoTest(ScratchDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public class User<TKey> where TKey : IEquatable<TKey>
        {
            public TKey Id { get; set; }
            public string UserName { get; set; }
        }

        public class CustomDbContext<TKey> : DataConnection where TKey : IEquatable<TKey>
        {
	        public ITable<User<TKey>> Users => GetTable<User<TKey>>();

        }

        public CustomDbContext<TKey> CreateContext<TKey>(bool delete = false) where TKey : IEquatable<TKey>
        {
	        var db = new CustomDbContext<TKey>();
            if (delete)
            {
				//db.Database.EnsureDeleted();
				//throw new NotImplementedException();
	            try
	            {
		            db.DropTable<User<TKey>>();
	            }
	            catch
	            {
		            //
	            }
            }

	        db.CreateTable<User<TKey>>();
            return db;
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public void CanUpdateNameGuid()
        {
            using (var db = CreateContext<Guid>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<Guid> { UserName = oldName, Id = Guid.NewGuid() };
				db.Insert(user);
                var newName = Guid.NewGuid().ToString();

				user.UserName = newName;
	            db.Update(user);

                Assert.Null(db.Users.SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Users.Single(u => u.UserName == newName));
            }
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public void CanUpdateNameString()
        {
            using (var db = CreateContext<string>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<string> { UserName = oldName, Id = Guid.NewGuid().ToString() };
	            db.Insert(user);

                var newName = Guid.NewGuid().ToString();
                user.UserName = newName;
	            db.Update(user);

                Assert.Null(db.Users.SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Users.Single(u => u.UserName == newName));
            }
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public void CanCreateUserInt()
        {
            using (var db = CreateContext<int>(true))
            {
                var user = new User<int>();
	            db.Insert(user);

                user.UserName = "Boo";
	            db.Update(user);

                var fetch = db.Users.First(u => u.UserName == "Boo");
                Assert.Equal(user, fetch);
            }
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public void CanUpdateNameInt()
        {
            using (var db = CreateContext<int>(true))
            {
                var oldName = Guid.NewGuid().ToString();
                var user = new User<int> { UserName = oldName };
                db.Insert(user);

                var newName = Guid.NewGuid().ToString();
                user.UserName = newName;
	            db.Update(user);

                Assert.Null(db.Users.SingleOrDefault(u => u.UserName == oldName));
                Assert.Equal(user, db.Users.Single(u => u.UserName == newName));
            }
        }
    }
}