using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class RoleManagerTest
    {
        [Fact]
        public void ConstructorThrowsWithNullStore()
        {
            Assert.Throws<ArgumentNullException>("store", () => new RoleManager<TestRole, string>(null));
        }

        [Fact]
        public void RolesQueryableFailWhenStoreNotImplemented()
        {
            var manager = new RoleManager<TestRole, string>(new NoopRoleStore());
            Assert.False(manager.SupportsQueryableRoles);
            Assert.Throws<NotSupportedException>(() => manager.Roles.Count());
        }


    }
}