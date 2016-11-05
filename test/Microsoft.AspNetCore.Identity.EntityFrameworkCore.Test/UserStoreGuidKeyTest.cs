// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using LinqToDB;
using LinqToDB.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class GuidUser : IdentityUser<Guid>
    {
        public GuidUser()
        {
            Id = Guid.NewGuid();
            UserName = Id.ToString();
        }
    }

    public class GuidRole : IdentityRole<Guid>
    {
        public GuidRole()
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
        }
    }

    public class UserStoreGuidTest : SqlStoreTestBase<GuidUser, GuidRole, Guid>
    {
        public UserStoreGuidTest(ScratchDatabaseFixture fixture)
            : base(fixture)
        {
        }

        public class ApplicationUserStore : UserStore<DataContext, TestDbContext, GuidUser, GuidRole, Guid>
        {
            public ApplicationUserStore() : base(new DefaultConnectionFactory<DataContext, TestDbContext>()) { }
        }

        public class ApplicationRoleStore : RoleStore<DataContext, TestDbContext, GuidRole, Guid>
        {
            public ApplicationRoleStore() : base(new DefaultConnectionFactory<DataContext, TestDbContext>()) { }
        }

        protected override void AddUserStore(IServiceCollection services, object context = null)
        {
            services.AddSingleton<IUserStore<GuidUser>>(new ApplicationUserStore());
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            services.AddSingleton<IRoleStore<GuidRole>>(new ApplicationRoleStore());
        }
    }
}