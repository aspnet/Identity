//// Copyright (c) .NET Foundation. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System;
//using LinqToDB.DataProvider.SQLite;
//using LinqToDB.Identity;

//namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.InMemory.Test
//{
//    public class InMemoryContext :
//        InMemoryContext<IdentityUser, IdentityRole, string>
//    {
//    }

//    public class InMemoryContext<TUser> :
//        InMemoryContext<TUser, IdentityRole, string>
//        where TUser : IdentityUser
//    {
//    }

//    public class InMemoryContext<TUser, TRole, TKey> : IdentityDataConnection<TUser, TRole, TKey>
//        where TUser : IdentityUser<TKey>
//        where TRole : IdentityRole<TKey>
//        where TKey : IEquatable<TKey>
//    {
//	    public InMemoryContext()
//		    : base(new SQLiteDataProvider(), "Data Source=:memory:;")
//	    {
//	    }
//    }

//    public abstract class InMemoryContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDataConnection<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
//        where TUser : IdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
//        where TRole : IdentityRole<TKey, TUserRole, TRoleClaim>
//        where TKey : IEquatable<TKey>
//        where TUserClaim : IdentityUserClaim<TKey>
//        where TUserRole : IdentityUserRole<TKey>
//        where TUserLogin : IdentityUserLogin<TKey>
//        where TRoleClaim : IdentityRoleClaim<TKey>
//        where TUserToken : IdentityUserToken<TKey>
//    {
//        public InMemoryContext() 
//			:base(new SQLiteDataProvider(), "Data Source=:memory:;")
//        { }

//    }
//}