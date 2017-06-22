// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity.Service.EntityFrameworkCore.InMemory.Test
{
    public class InMemoryContext : InMemoryContext<IdentityUser, IdentityClientApplication>
    {
        public InMemoryContext(DbContextOptions options) : base(options)
        { }
    }

    public class InMemoryContext<TUser, TApplication> :
        InMemoryContext<TUser, IdentityRole, string, TApplication, string>
        where TUser : IdentityUser
        where TApplication : IdentityClientApplication
    {
        public InMemoryContext(DbContextOptions options) : base(options)
        { }
    }

    public class InMemoryContext<TUser, TRole, TUserKey, TApplication, TApplicationKey> : IdentityClientApplicationsDbContext<TUser, TRole, TUserKey, TApplication, TApplicationKey>
        where TUser : IdentityUser<TUserKey>
        where TRole : IdentityRole<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TApplication : IdentityClientApplication<TApplicationKey>
        where TApplicationKey : IEquatable<TApplicationKey>
    {
        public InMemoryContext(DbContextOptions options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Scratch");
        }
    }

    public abstract class InMemoryContext<
        TUser,
        TRole,
        TUserKey,
        TUserClaim,
        TUserRole,
        TUserLogin,
        TRoleClaim,
        TUserToken,
        TApplication,
        TScope,
        TApplicationClaim,
        TRedirectUri,
        TApplicationKey> :
        IdentityClientApplicationsDbContext<TUser, TRole, TUserKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TApplication, TScope, TApplicationClaim, TRedirectUri, TApplicationKey>
        where TUser : IdentityUser<TUserKey>
        where TRole : IdentityRole<TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TUserClaim : IdentityUserClaim<TUserKey>
        where TUserRole : IdentityUserRole<TUserKey>
        where TUserLogin : IdentityUserLogin<TUserKey>
        where TRoleClaim : IdentityRoleClaim<TUserKey>
        where TUserToken : IdentityUserToken<TUserKey>
        where TApplication : IdentityClientApplication<TApplicationKey, TScope, TApplicationClaim, TRedirectUri>
        where TScope : IdentityClientApplicationScope<TApplicationKey>
        where TApplicationClaim : IdentityClientApplicationClaim<TApplicationKey>
        where TRedirectUri : IdentityClientApplicationRedirectUri<TApplicationKey>
        where TApplicationKey : IEquatable<TApplicationKey>
    {
        public InMemoryContext(DbContextOptions options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Scratch");
        }
    }
}