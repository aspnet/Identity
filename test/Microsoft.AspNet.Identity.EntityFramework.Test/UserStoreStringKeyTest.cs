// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Identity.Test;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using System;
using Xunit;

namespace Microsoft.AspNet.Identity.EntityFramework.Test
{
    [TestCaseOrderer("Microsoft.AspNet.Identity.Test.PriorityOrderer", "Microsoft.AspNet.Identity.EntityFramework.Test")]
    public class StringUser : IdentityUser
    {
        public StringUser()
        {
            Id = Guid.NewGuid().ToString();
            UserName = Id;
        }
    }

    public class StringRole : IdentityRole<string>
    {
        public StringRole()
        {
            Id = Guid.NewGuid().ToString();
            Name = Id;
        }
    }

    [TestCaseOrderer("Microsoft.AspNet.Identity.Test.PriorityOrderer", "Microsoft.AspNet.Identity.EntityFramework.Test")]
    public class UserStoreStringKeyTest : UserStoreTestBase<StringUser, StringRole, string>
    {
        public override string ConnectionString
        {
            get
            {
                return @"Server=(localdb)\v11.0;Database=SqlUserStoreStringTest;Trusted_Connection=True;";
            }
        }

        public override UserManager<StringUser> CreateManager(ApplicationDbContext context)
        {
            return MockHelpers.CreateManager(() => new UserStore<StringUser, StringRole, ApplicationDbContext, string>(context));
        }

        public override RoleManager<StringRole> CreateRoleManager(ApplicationDbContext context)
        {
            var services = new ServiceCollection();
            services.AddIdentity<StringUser, StringRole>().AddRoleStore(() => new RoleStore<StringRole, ApplicationDbContext, string>(context));
            return services.BuildServiceProvider().GetService<RoleManager<StringRole>>();
        }
    }
}