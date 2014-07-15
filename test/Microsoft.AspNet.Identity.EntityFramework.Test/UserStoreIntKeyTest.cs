// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity.Test;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using System;
using Xunit;

namespace Microsoft.AspNet.Identity.EntityFramework.Test
{
    public class IntUser : IdentityUser<int>
    {
        public IntUser()
        {
            UserName = Guid.NewGuid().ToString();
        }
    }

    public class IntRole : IdentityRole<int>
    {
        public IntRole()
        {
            Name = Guid.NewGuid().ToString();
        }
    }

    [TestCaseOrderer("Microsoft.AspNet.Identity.Test.PriorityOrderer", "Microsoft.AspNet.Identity.EntityFramework.Test")]
    public class UserStoreIntTest : UserStoreTestBase<IntUser, IntRole, int>
    {
        public override string ConnectionString
        {
            get
            {
                return @"Server=(localdb)\v11.0;Database=SqlUserStoreIntTest;Trusted_Connection=True;";
            }
        }

        public override UserManager<IntUser> CreateManager(ApplicationDbContext context)
        {
            return MockHelpers.CreateManager(() => new UserStore<IntUser, IntRole, ApplicationDbContext, int>(context));
        }

        public override RoleManager<IntRole> CreateRoleManager(ApplicationDbContext context)
        {
            var services = new ServiceCollection();
            services.AddIdentity<IntUser, IntRole>(b => b.AddRoleStore(() => new RoleStore<IntRole, ApplicationDbContext, int>(context)));
            return services.BuildServiceProvider().GetService<RoleManager<IntRole>>();
        }
    }
}