// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework.InMemory.Tests
{
    public class InMemoryContext :
        InMemoryContext<IdentityUser, IdentityRole, string>
    {
    }

    public class InMemoryContext<TUser> :
        InMemoryContext<TUser, IdentityRole, string>
        where TUser : IdentityUser
    {
    }

    public class InMemoryContext<TUser, TRole, TKey> : IdentityDbContext<TUser,TRole,TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public InMemoryContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Want fresh in memory store for tests always for now
            optionsBuilder.UseInMemoryDatabase(persist: false);
        }
    }
}