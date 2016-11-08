// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Identity.Test;
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

        public class ApplicationUserStore : UserStore<DataContext, DataConnection, GuidUser, GuidRole, Guid>
        {
            public ApplicationUserStore(IConnectionFactory<DataContext, DataConnection> factory) : base(factory) { }
        }

        public class ApplicationRoleStore : RoleStore<DataContext, DataConnection, GuidRole, Guid>
        {
            public ApplicationRoleStore(IConnectionFactory<DataContext, DataConnection> factory) : base(factory) { }
        }

        protected override void AddUserStore(IServiceCollection services, TestConnectionFactory context = null)
        {
            services.AddSingleton<IUserStore<GuidUser>>(new ApplicationUserStore(context ?? CreateTestContext()));
        }

        protected override void AddRoleStore(IServiceCollection services, TestConnectionFactory context = null)
        {
            services.AddSingleton<IRoleStore<GuidRole>>(new ApplicationRoleStore(context ?? CreateTestContext()));
        }
    }
}