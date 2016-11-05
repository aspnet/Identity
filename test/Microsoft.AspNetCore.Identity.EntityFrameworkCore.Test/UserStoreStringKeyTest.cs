// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using LinqToDB;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class StringUser : IdentityUser
    {
		[Column(DataType = DataType.VarChar, Length = 36, IsPrimaryKey = true, SkipOnUpdate = true, CanBeNull = false)]
	    public override string Id { get; set; }

	    public StringUser()
        {
            Id = Guid.NewGuid().ToString();
            UserName = Id;
        }
    }

    public class StringRole : IdentityRole<string>
    {
		[Column(DataType = DataType.VarChar, Length = 36, IsPrimaryKey = true, SkipOnUpdate = true, CanBeNull = false)]
	    public override string Id { get; set; }

        public StringRole()
        {
            Id = Guid.NewGuid().ToString();
            Name = Id;
        }
    }

    public class UserStoreStringKeyTest : SqlStoreTestBase<StringUser, StringRole, string>
    {
        public UserStoreStringKeyTest(ScratchDatabaseFixture fixture)
            : base(fixture)
        {
        }
    }
}